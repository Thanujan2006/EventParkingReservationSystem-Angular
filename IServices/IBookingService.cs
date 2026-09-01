using EventParkingReservationSystem.API.DTOs.Bookings;

namespace EventParkingReservationSystem.API.Services;

public interface IBookingService
{
    Task<BookingResponseDto> CreateAsync(int customerId, CreateBookingDto dto);
    Task<BookingResponseDto> GetAsync(int bookingId);
    Task<IReadOnlyList<BookingResponseDto>> GetCustomerHistoryAsync(int customerId);
    Task<IReadOnlyList<BookingResponseDto>> GetByEventAsync(int eventId);
    Task<HoldStatusDto> GetHoldStatusAsync(int bookingId);
    Task<BookingResponseDto> AddSeatsAsync(int bookingId, IReadOnlyList<int> seatIds);
    Task<BookingResponseDto> AddParkingAsync(int bookingId, int parkingSlotId);
    Task<BookingResponseDto> RemoveParkingAsync(int bookingId);
    Task CancelAsync(int bookingId);
    Task<int> ExpirePendingBookingsAsync(CancellationToken cancellationToken = default);
}
