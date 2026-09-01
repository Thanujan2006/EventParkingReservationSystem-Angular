using EventParkingReservationSystem.API.DTOs.Payments;
using EventParkingReservationSystem.API.Enums;
using EventParkingReservationSystem.API.Exceptions;
using EventParkingReservationSystem.API.IRepositories;
using EventParkingReservationSystem.API.IServices;
using EventParkingReservationSystem.API.Models;
using EventParkingReservationSystem.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.API.Services;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _uow;
    private readonly INotificationService _notifications;

    public PaymentService(IUnitOfWork uow, INotificationService notifications)
    {
        _uow = uow;
        _notifications = notifications;
    }

    public async Task<PaymentSummaryDto> GetSummaryAsync(int bookingId)
    {
        var booking = await _uow.Bookings.Query().AsNoTracking()
            .Include(x => x.Payment)
            .FirstOrDefaultAsync(x => x.BookingId == bookingId)
            ?? throw new ApiException(404, "Booking not found.");

        return new PaymentSummaryDto(
            booking.BookingId,
            booking.BookingNumber,
            booking.TotalAmount,
            booking.Payment is not null && booking.Payment.Status == PaymentStatus.Completed,
            booking.Status.ToString());
    }

    public async Task<PaymentResponseDto> PayAsync(int bookingId)
    {
        await using var tx = await _uow.BeginTransactionAsync();

        var booking = await _uow.Bookings.Query()
            .Include(x => x.Payment)
            .Include(x => x.BookingSeats).ThenInclude(x => x.Seat)
            .Include(x => x.ParkingReservations).ThenInclude(x => x.ParkingSlot)
            .FirstOrDefaultAsync(x => x.BookingId == bookingId)
            ?? throw new ApiException(404, "Booking not found.");

        if (booking.Payment is not null)
            throw new ApiException(409, "Payment already recorded for this booking.");

        if (booking.Status != BookingStatus.Pending)
            throw new ApiException(400, $"Booking is {booking.Status} and cannot be paid.");

        if (booking.HoldExpiresAtUtc.HasValue && booking.HoldExpiresAtUtc <= DateTime.UtcNow)
            throw new ApiException(400, "This booking has expired and cannot be paid for.");

        var payment = new Payment
        {
            BookingId = booking.BookingId,
            Amount = booking.TotalAmount,
            Status = PaymentStatus.Completed,
            PaidAtUtc = DateTime.UtcNow
        };

        await _uow.Payments.AddAsync(payment);

        booking.Status = BookingStatus.Confirmed;
        booking.HoldExpiresAtUtc = null;
        booking.ConfirmedAtUtc = DateTime.UtcNow;
        booking.UpdatedAtUtc = DateTime.UtcNow;

        foreach (var item in booking.BookingSeats)
            item.Seat.Status = SeatStatus.Booked;

        foreach (var reservation in booking.ParkingReservations.Where(x => x.IsActive))
            reservation.ParkingSlot.Status = ParkingSlotStatus.Reserved;

        await _uow.SaveChangesAsync();

        await _notifications.CreateAsync(
            booking.CustomerId,
            NotificationType.PaymentCompleted,
            $"Payment of {payment.Amount:0.00} completed for booking {booking.BookingNumber}.");

        await _notifications.CreateAsync(
            booking.CustomerId,
            NotificationType.BookingConfirmed,
            $"Booking {booking.BookingNumber} is confirmed.");

        await tx.CommitAsync();

        return new PaymentResponseDto(
            payment.PaymentId, payment.BookingId, payment.Amount,
            payment.Status.ToString(), payment.PaidAtUtc);
    }

    public async Task<IReadOnlyList<PaymentResponseDto>> GetCustomerHistoryAsync(int customerId)
        => await _uow.Payments.Query().AsNoTracking()
            .Where(x => x.Booking.CustomerId == customerId)
            .OrderByDescending(x => x.PaidAtUtc)
            .Select(x => new PaymentResponseDto(
                x.PaymentId, x.BookingId, x.Amount, x.Status.ToString(), x.PaidAtUtc))
            .ToListAsync();

    public async Task<(string FileName, string Content)> GetReceiptAsync(int paymentId)
    {
        var p = await _uow.Payments.Query().AsNoTracking()
            .Include(x => x.Booking).ThenInclude(x => x.Customer)
            .Include(x => x.Booking).ThenInclude(x => x.Event)
            .FirstOrDefaultAsync(x => x.PaymentId == paymentId)
            ?? throw new ApiException(404, "Payment not found.");

        var content = $"""
EVENT & PARKING RESERVATION SYSTEM
PAYMENT RECEIPT

Payment ID: {p.PaymentId}
Booking Number: {p.Booking.BookingNumber}
Customer: {p.Booking.Customer.Name}
Event: {p.Booking.Event.Name}
Amount: {p.Amount:0.00}
Status: {p.Status}
Paid At (UTC): {p.PaidAtUtc:u}
""";

        return ($"receipt-{p.Booking.BookingNumber}.txt", content);
    }
}
