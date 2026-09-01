using EventParkingReservationSystem.API.Exceptions;
using EventParkingReservationSystem.API.IRepositories;
using EventParkingReservationSystem.API.IServices;
using EventParkingReservationSystem.API.Models;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DTOs.Events;
using WebApplication1.Enums;
using WebApplication1.Repositories;

namespace WebApplication1.Services;

//public class EventService : IEventService
//{
//    private readonly IUnitOfWork _uow;
//    private readonly INotificationService _notifications;
//    public EventService(IUnitOfWork uow, INotificationService notifications)
//    {
//        _uow = uow;
//        _notifications = notifications;
//    }

//    public async Task<IReadOnlyList<EventResponseDto>> GetAllAsync(
//    string? name, DateOnly? date, int? venueId, int? categoryID)
//    {
//        var query = _uow.EventCategories.Query().AsNoTracking()
//        .Include(x => x.Venue)
//        .ThenInclude(x => x.EventCetegory)
//        .AsQueryable();
//        if (!string.IsNullOrWhiteSpace(name))
//        {
//            query = query.Where(x => x.Name.Contains(name));
//        }
//        if (date.HasValue)
//        {
//            query = query.Where(x => x.EventDate == date.Value);
//        }
//        if (venueId.HasValue)
//        {
//            query = query.Where(x => x.VenueId == venueId.Value);
//        }
//        if (categoryID.HasValue)
//        {
//            query = query.Where(x => x.EventCategoryId == categoryId.Value);
//        }

//        return await query
//            .OrderBy(x => x.EventDate).ThenBy(x => x.StartTime)
//            .Select(x => new EventResponseDto(
//                x.EventId,
//                x.Name,
//                x.VenueId,
//                x.Venue.Name,
//                x.EventCategoryId,
//                x.EventCategory.Name,
//                x.EventDate,
//                x.StartTime,
//                x.EndTime,
//                x.TicketPrice,
//                x.ParkingFee,
//                x.Capacity
//            ))
//            .ToListAsync();
//    }
//    public async Task<EventResponseDto> GetAsync(int id)
//    {
//        var x = await _uow.Events.Query().AsNoTracking()
//            .Include(e => e.Venue)
//            .ThenInclude(e => e.EventCategory)
//            .FirstOrDefaultAsync(x => x.EventId == id)
//            ?? throw new ApiException(404., "Event not found.");
//        return Map(x);
//    }

//    public async Task<EventResponseDto> CreateAsync(EventUpsertDto dto)
//    {
//        await ValidateAsync(dto, null);
//        await using var tx = await _uow.BeginTransactionAsync();
//        await ValodateOverlapAsync(dto, null);

//        var evt = new Event
//        {
//            Name = dto.Name Trim(),
//            VenueId = dto.VenueId,
//            EventCategoryId = dto.EventCategoryId,
//            EventDate = dto.EventDate,
//            StartTime = dto.StartTime,
//            EndTime = dto.EndTime,
//            TicketPrice = dto.TicketPrice,
//            Capacity = dto.Capacity,
//            ParkingFee = dto.ParkingFee
//        };
//        await _uow.Events.AddAsync(evt);
//        await _uow.SaveChangesAsync();
//        await tx.CommitAsync();

//        return await GetAsync(evt.EventId);
//    }
//    public async Task<EventResponseDto> UpdateAsync(int id, EventUpsertDto dto)
//    {
//        var evt = await _uow.EventCategories.GetByIdAsync(id)
//             ?? throw new ApiException(404, "Event not found.");
//        await ValidateAsync(dto, id);
//        var activeBookings = await _uow.Bookings.Query()
//            .CountAsync(x => x.EventId == id &&
//            (x.Status == BookingStatus.Pending || x.Status == BookingStatus.Confirmed));

//        var bookedSeatCount = await _uow.Bookings.Query()
//            .Where(bs => bs.EventId == id &&
//            (bs.Status == BookingStatus.Pending || bs.Status == BookingStatus.Confirmed))
//            .Select(bs => bs.SeatId)
//            .Distinct()
//            .CountAsync();
//        if (dto.Capacity < bookedSeatCount)
//            throw new ApiException(400, $"Capacity cannot be reduced below already booked/held seats ({bookedSeatCount}).");

//        if (activeBookings > 0 && dto.TicketPrice != evt.TicketPrice)
//            throw new ApiException(409, "Ticket price cannot be changed after active bookings exist.");
//        await using var tx = await _uow.BeginTransactionAsync();
//        await ValidateOverlapAsync(dto, id);

//        var significantChange =
//            evt.EventDate != dto.EventDate ||
//            evt.StartTime != dto.StartTime ||
//            evt.EndTime != dto.EndTime ||
//            evt.VenueId != dto.VenueId;

//        evt.Name = dto.Name.Trim();
//        evt.VenueId = dto.VenueId;
//        evt.EventCategoryId = dto.EventCategoryId;
//        evt.EventDate = dto.EventDate;
//        evt.StartTime = dto.StartTime;
//        evt.EndTime = dto.EndTime;
//        evt.TicketPrice = dto.TicketPrice;
//        evt.Capacity = dto.Capacity;
//        evt.ParkingFee = dto.ParkingFee;
//        evt.UpdatedAtUtc = DateTime.UtcNow;

//        await _uow.SaveChangesAsync();

//        if (significantChange)
//        {
//            var customerIds = await _uow.Bookings.Query()
//                .Where(x => x.EventId == id && x.Status == BookingStatus.Confirmed)
//                .Select(x => x.CustomerId)
//                .Distinct()
//                .ToListAsync();

//            foreach (var customerId in customerIds)
//                await _notifications.CreateAsync(customerId, NotificationType.EventUpdate,
//                    $"Event '{evt.Name}' details were updated.");
//        }

//        await tx.CommitAsync();
//        return await GetAsync(id);
//    }
//    public async Task DeleteAsync(int id)
//    {
//        var evt = await _uow.Events.GetByIdAsync(id)
//            ?? throw new ApiException(404, "Event not found.");

//        if (await _uow.Bookings.Query().AnyAsync(x => x.EventId == id &&
//            (x.Status == BookingStatus.Pending || x.Status == BookingStatus.Confirmed)))
//            throw new ApiException(409, "Cannot delete an event with active bookings.");

//        _uow.Events.Remove(evt);
//        await _uow.SaveChangesAsync();
//    }

//    private async Task ValidateAsync(EventUpsertDto dto, int? editingId)
//    {
//        if (dto.StartTime >= dto.EndTime)
//            throw new ApiException(400, "End time must be after start time.");

//        if (dto.EventDate < DateOnly.FromDateTime(DateTime.UtcNow))
//            throw new ApiException(400, "Event date cannot be in the past.");

//        var venue = await _uow.Venues.GetByIdAsync(dto.VenueId)
//            ?? throw new ApiException(400, "Selected venue does not exist.");

//        if (await _uow.EventCategories.GetByIdAsync(dto.EventCategoryId) is null)
//            throw new ApiException(400, "Selected category does not exist.");

//        if (dto.Capacity > venue.TotalCapacity)
//            throw new ApiException(400, $"Event capacity cannot exceed venue capacity ({venue.TotalCapacity}).");
//    }

//    private async Task ValidateAsync(EventUpsertDto dto, int? editingId)
//    {
//        if (dto.StartTime >= dto.EndTime)
//            throw new ApiException(400, "End time must be after start time.");

//        if (dto.EventDate < DateOnly.FromDateTime(DateTime.UtcNow))
//            throw new ApiException(400, "Event date cannot be in the past.");

//        var venue = await _uow.Venues.GetByIdAsync(dto.VenueId)
//            ?? throw new ApiException(400, "Selected venue does not exist.");

//        if (await _uow.EventCategories.GetByIdAsync(dto.EventCategoryId) is null)
//            throw new ApiException(400, "Selected category does not exist.");

//        if (dto.Capacity > venue.TotalCapacity)
//            throw new ApiException(400, $"Event capacity cannot exceed venue capacity ({venue.TotalCapacity}).");
//    }
//    private async Task ValidateOverlapAsync(EventUpsertDto dto, int? editingId)
//    {
//        var overlaps = await _uow.Events.Query().AnyAsync(x =>
//            x.VenueId == dto.VenueId &&
//            x.EventDate == dto.EventDate &&
//            (!editingId.HasValue || x.EventId != editingId.Value) &&
//            x.StartTime < dto.EndTime &&
//            x.EndTime > dto.StartTime);

//        if (overlaps)
//            throw new ApiException(409, "Venue is already booked for an overlapping time slot.");
//    }

//    private static EventResponseDto Map(Event x)
//        => new(x.EventId,
//            x.Name,
//            x.VenueId,
//            x.Venue.Name,
//            x.EventCategoryId,
//            x.EventCategory.Name,
//            x.EventDate,
//            x.StartTime,
//            x.EndTime,
//            x.TicketPrice,
//            x.ParkingFee,
//            x.Capacity);
//}









