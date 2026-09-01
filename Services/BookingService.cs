using EventParkingReservationSystem.API.DTOs.Bookings;
using EventParkingReservationSystem.API.Enums;
using EventParkingReservationSystem.API.Exceptions;
using EventParkingReservationSystem.API.Helpers;
using EventParkingReservationSystem.API.IRepositories;
using EventParkingReservationSystem.API.Models;
using EventParkingReservationSystem.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.API.Services;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _uow;
    private readonly IConfiguration _config;
    private readonly INotificationService _notifications;

    public BookingService(IUnitOfWork uow, IConfiguration config, INotificationService notifications)
    {
        _uow = uow;
        _config = config;
        _notifications = notifications;
    }

    public async Task<BookingResponseDto> CreateAsync(int customerId, CreateBookingDto dto)
    {
        if (dto.SeatIds is null || dto.SeatIds.Count == 0)
            throw new ApiException(400, "A booking must contain at least one seat.");

        if (dto.SeatIds.Distinct().Count() != dto.SeatIds.Count)
            throw new ApiException(400, "The same seat cannot be selected twice.");

        var customer = await _uow.Customers.GetByIdAsync(customerId)
            ?? throw new ApiException(404, "Customer not found.");

        if (customer.Status != CustomerStatus.Active)
            throw new ApiException(403, "Account is deactivated.");
        if (!customer.EmailVerified)
            throw new ApiException(403, "Please verify your email before making a booking.");

        var evt = await _uow.Events.GetByIdAsync(dto.EventId)
            ?? throw new ApiException(404, "Event not found.");

        if (evt.EventDate < DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ApiException(400, "This event is no longer bookable.");

        await using var tx = await _uow.BeginTransactionAsync();

        var seats = await _uow.Seats.Query()
            .Where(x => x.EventId == dto.EventId && dto.SeatIds.Contains(x.SeatId))
            .ToListAsync();

        if (seats.Count != dto.SeatIds.Count)
            throw new ApiException(404, "One or more selected seats do not belong to this event.");

        if (seats.Any(x => x.Status != SeatStatus.Available))
            throw new ApiException(409, "One or more selected seats were just booked or held by another customer.");

        ParkingSlot? slot = null;
        if (dto.ParkingSlotId.HasValue)
        {
            slot = await _uow.ParkingSlots.Query()
                .FirstOrDefaultAsync(x =>
                    x.ParkingSlotId == dto.ParkingSlotId.Value &&
                    x.EventId == dto.EventId);

            if (slot is null)
                throw new ApiException(404, "Selected parking slot does not belong to this event.");
            if (slot.Status != ParkingSlotStatus.Available)
                throw new ApiException(409, "This parking slot was just reserved by another customer.");
        }

        var holdMinutes = _config.GetValue<int?>("BookingSettings:HoldMinutes") ?? 15;
        var booking = new Booking
        {
            BookingNumber = BookingNumberGenerator.NewNumber(),
            CustomerId = customerId,
            EventId = dto.EventId,
            Status = BookingStatus.Pending,
            HoldExpiresAtUtc = DateTime.UtcNow.AddMinutes(holdMinutes),
            TotalAmount = seats.Sum(x => x.Price ?? evt.TicketPrice) + (slot is null ? 0 : evt.ParkingFee)
        };

        await _uow.Bookings.AddAsync(booking);
        await _uow.SaveChangesAsync();

        foreach (var seat in seats)
        {
            seat.Status = SeatStatus.Held;
            await _uow.BookingSeats.AddAsync(new BookingSeat
            {
                BookingId = booking.BookingId,
                SeatId = seat.SeatId
            });
        }

        if (slot is not null)
        {
            slot.Status = ParkingSlotStatus.Held;
            await _uow.ParkingReservations.AddAsync(new ParkingReservation
            {
                BookingId = booking.BookingId,
                ParkingSlotId = slot.ParkingSlotId,
                FeeAtReservation = evt.ParkingFee,
                IsActive = true
            });
        }

        await _uow.SaveChangesAsync();
        await tx.CommitAsync();

        return await GetAsync(booking.BookingId);
    }

    public async Task<BookingResponseDto> GetAsync(int bookingId)
    {
        var booking = await BaseBookingQuery().AsNoTracking()
            .FirstOrDefaultAsync(x => x.BookingId == bookingId)
            ?? throw new ApiException(404, "Booking not found.");

        return Map(booking);
    }

    public async Task<IReadOnlyList<BookingResponseDto>> GetCustomerHistoryAsync(int customerId)
    {
        var items = await BaseBookingQuery().AsNoTracking()
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync();

        return items.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<BookingResponseDto>> GetByEventAsync(int eventId)
    {
        var items = await BaseBookingQuery().AsNoTracking()
            .Where(x => x.EventId == eventId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync();

        return items.Select(Map).ToList();
    }

    public async Task<HoldStatusDto> GetHoldStatusAsync(int bookingId)
    {
        var booking = await _uow.Bookings.GetByIdAsync(bookingId)
            ?? throw new ApiException(404, "Booking not found.");

        var remaining = booking.HoldExpiresAtUtc.HasValue
            ? Math.Max(0, (int)(booking.HoldExpiresAtUtc.Value - DateTime.UtcNow).TotalSeconds)
            : 0;

        var expired = booking.Status == BookingStatus.Expired ||
                      (booking.Status == BookingStatus.Pending &&
                       booking.HoldExpiresAtUtc.HasValue &&
                       booking.HoldExpiresAtUtc <= DateTime.UtcNow);

        return new HoldStatusDto(
            booking.BookingId, booking.Status.ToString(),
            booking.HoldExpiresAtUtc, remaining, expired);
    }

    public async Task<BookingResponseDto> AddSeatsAsync(int bookingId, IReadOnlyList<int> seatIds)
    {
        if (seatIds.Count == 0)
            throw new ApiException(400, "At least one seat is required.");
        if (seatIds.Distinct().Count() != seatIds.Count)
            throw new ApiException(400, "The same seat cannot be selected twice.");

        await using var tx = await _uow.BeginTransactionAsync();

        var booking = await _uow.Bookings.Query()
            .Include(x => x.Event)
            .FirstOrDefaultAsync(x => x.BookingId == bookingId)
            ?? throw new ApiException(404, "Booking not found.");

        EnsurePendingNotExpired(booking);

        var existingIds = await _uow.BookingSeats.Query()
            .Where(x => x.BookingId == bookingId)
            .Select(x => x.SeatId)
            .ToListAsync();

        if (seatIds.Any(existingIds.Contains))
            throw new ApiException(400, "A selected seat is already attached to this booking.");

        var seats = await _uow.Seats.Query()
            .Where(x => x.EventId == booking.EventId && seatIds.Contains(x.SeatId))
            .ToListAsync();

        if (seats.Count != seatIds.Count)
            throw new ApiException(404, "One or more selected seats do not belong to this event.");
        if (seats.Any(x => x.Status != SeatStatus.Available))
            throw new ApiException(409, "One or more selected seats are no longer available.");

        foreach (var seat in seats)
        {
            seat.Status = SeatStatus.Held;
            await _uow.BookingSeats.AddAsync(new BookingSeat
            {
                BookingId = bookingId,
                SeatId = seat.SeatId
            });
        }

        booking.TotalAmount += seats.Sum(x => x.Price ?? booking.Event.TicketPrice);
        booking.UpdatedAtUtc = DateTime.UtcNow;
        await _uow.SaveChangesAsync();
        await tx.CommitAsync();

        return await GetAsync(bookingId);
    }

    public async Task<BookingResponseDto> AddParkingAsync(int bookingId, int parkingSlotId)
    {
        await using var tx = await _uow.BeginTransactionAsync();

        var booking = await _uow.Bookings.Query()
            .Include(x => x.Event)
            .FirstOrDefaultAsync(x => x.BookingId == bookingId)
            ?? throw new ApiException(404, "Booking not found.");

        EnsurePendingNotExpired(booking);

        if (await _uow.ParkingReservations.Query().AnyAsync(x => x.BookingId == bookingId && x.IsActive))
            throw new ApiException(409, "This booking already has a parking reservation.");

        var slot = await _uow.ParkingSlots.Query()
            .FirstOrDefaultAsync(x => x.ParkingSlotId == parkingSlotId && x.EventId == booking.EventId)
            ?? throw new ApiException(404, "Parking slot not found for this event.");

        if (slot.Status != ParkingSlotStatus.Available)
            throw new ApiException(409, "Parking slot is already held or reserved.");

        slot.Status = ParkingSlotStatus.Held;
        await _uow.ParkingReservations.AddAsync(new ParkingReservation
        {
            BookingId = bookingId,
            ParkingSlotId = slot.ParkingSlotId,
            FeeAtReservation = booking.Event.ParkingFee,
            IsActive = true
        });

        booking.TotalAmount += booking.Event.ParkingFee;
        booking.UpdatedAtUtc = DateTime.UtcNow;

        await _uow.SaveChangesAsync();
        await tx.CommitAsync();
        return await GetAsync(bookingId);
    }

    public async Task<BookingResponseDto> RemoveParkingAsync(int bookingId)
    {
        await using var tx = await _uow.BeginTransactionAsync();

        var booking = await _uow.Bookings.GetByIdAsync(bookingId)
            ?? throw new ApiException(404, "Booking not found.");
        EnsurePendingNotExpired(booking);

        var reservation = await _uow.ParkingReservations.Query()
            .Include(x => x.ParkingSlot)
            .FirstOrDefaultAsync(x => x.BookingId == bookingId && x.IsActive)
            ?? throw new ApiException(404, "No active parking reservation found.");

        reservation.IsActive = false;
        reservation.ReleasedAtUtc = DateTime.UtcNow;
        reservation.ParkingSlot.Status = ParkingSlotStatus.Available;

        booking.TotalAmount = Math.Max(0, booking.TotalAmount - reservation.FeeAtReservation);
        booking.UpdatedAtUtc = DateTime.UtcNow;

        await _uow.SaveChangesAsync();
        await tx.CommitAsync();
        return await GetAsync(bookingId);
    }

    public async Task CancelAsync(int bookingId)
    {
        await using var tx = await _uow.BeginTransactionAsync();

        var booking = await _uow.Bookings.Query()
            .Include(x => x.BookingSeats).ThenInclude(x => x.Seat)
            .Include(x => x.ParkingReservations).ThenInclude(x => x.ParkingSlot)
            .FirstOrDefaultAsync(x => x.BookingId == bookingId)
            ?? throw new ApiException(404, "Booking not found.");

        if (booking.Status == BookingStatus.Cancelled)
            return;
        if (booking.Status == BookingStatus.Expired)
            throw new ApiException(400, "This booking has already expired.");

        foreach (var item in booking.BookingSeats)
            item.Seat.Status = SeatStatus.Available;

        foreach (var reservation in booking.ParkingReservations.Where(x => x.IsActive))
        {
            reservation.IsActive = false;
            reservation.ReleasedAtUtc = DateTime.UtcNow;
            reservation.ParkingSlot.Status = ParkingSlotStatus.Available;
        }

        booking.Status = BookingStatus.Cancelled;
        booking.HoldExpiresAtUtc = null;
        booking.CancelledAtUtc = DateTime.UtcNow;
        booking.UpdatedAtUtc = DateTime.UtcNow;

        await _uow.SaveChangesAsync();
        await _notifications.CreateAsync(booking.CustomerId, NotificationType.BookingCancelled,
            $"Booking {booking.BookingNumber} was cancelled.");
        await tx.CommitAsync();
    }

    public async Task<int> ExpirePendingBookingsAsync(CancellationToken cancellationToken = default)
    {
        await using var tx = await _uow.BeginTransactionAsync(cancellationToken: cancellationToken);

        var expired = await _uow.Bookings.Query()
            .Include(x => x.BookingSeats).ThenInclude(x => x.Seat)
            .Include(x => x.ParkingReservations).ThenInclude(x => x.ParkingSlot)
            .Where(x => x.Status == BookingStatus.Pending &&
                        x.HoldExpiresAtUtc != null &&
                        x.HoldExpiresAtUtc <= DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        foreach (var booking in expired)
        {
            foreach (var item in booking.BookingSeats)
                item.Seat.Status = SeatStatus.Available;

            foreach (var reservation in booking.ParkingReservations.Where(x => x.IsActive))
            {
                reservation.IsActive = false;
                reservation.ReleasedAtUtc = DateTime.UtcNow;
                reservation.ParkingSlot.Status = ParkingSlotStatus.Available;
            }

            booking.Status = BookingStatus.Expired;
            booking.HoldExpiresAtUtc = null;
            booking.UpdatedAtUtc = DateTime.UtcNow;

            await _uow.Notifications.AddAsync(new Notification
            {
                CustomerId = booking.CustomerId,
                Type = NotificationType.BookingExpired,
                Message = $"Booking {booking.BookingNumber} expired because payment was not completed in time."
            });
        }

        await _uow.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);
        return expired.Count;
    }

    private IQueryable<Booking> BaseBookingQuery()
        => _uow.Bookings.Query()
            .Include(x => x.Event)
            .Include(x => x.BookingSeats).ThenInclude(x => x.Seat)
            .Include(x => x.ParkingReservations).ThenInclude(x => x.ParkingSlot);

    private static BookingResponseDto Map(Booking x)
    {
        var seats = x.BookingSeats
            .Select(bs => new BookingSeatDto(
                bs.SeatId, bs.Seat.SeatNumber, bs.Seat.Price ?? x.Event.TicketPrice))
            .OrderBy(s => s.SeatNumber)
            .ToList();

        var p = x.ParkingReservations
            .Where(r => r.IsActive)
            .OrderByDescending(r => r.ParkingReservationId)
            .FirstOrDefault();

        ParkingReservationDto? parking = p is null
            ? null
            : new ParkingReservationDto(p.ParkingSlotId, p.ParkingSlot.SlotNumber, p.FeeAtReservation);

        return new BookingResponseDto(
            x.BookingId, x.BookingNumber, x.CustomerId, x.EventId, x.Event.Name,
            x.Status.ToString(), x.TotalAmount, x.HoldExpiresAtUtc,
            seats, parking, x.CreatedAtUtc);
    }

    private static void EnsurePendingNotExpired(Booking booking)
    {
        if (booking.Status != BookingStatus.Pending)
            throw new ApiException(400, "Only a pending booking can be modified.");
        if (booking.HoldExpiresAtUtc.HasValue && booking.HoldExpiresAtUtc <= DateTime.UtcNow)
            throw new ApiException(400, "This booking has expired. Please create a new booking.");
    }
}
