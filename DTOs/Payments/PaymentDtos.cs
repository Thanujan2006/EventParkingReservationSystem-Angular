
namespace EventParkingReservationSystem.API.DTOs.Payments;
public record PaymentSummaryDto(int BookingId, string BookingNumber, decimal AmountDue, bool Paid, string BookingStatus);
public record PaymentResponseDto(int PaymentId, int BookingId, decimal Amount, string Status, DateTime PaidAtUtc);
