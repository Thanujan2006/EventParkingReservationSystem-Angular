using EventParkingReservationSystem.API.DTOs.Payments;

namespace EventParkingReservationSystem.API.Services;

public interface IPaymentService
{
    Task<PaymentSummaryDto> GetSummaryAsync(int bookingId);
    Task<PaymentResponseDto> PayAsync(int bookingId);
    Task<IReadOnlyList<PaymentResponseDto>> GetCustomerHistoryAsync(int customerId);
    Task<(string FileName, string Content)> GetReceiptAsync(int paymentId);
}
