using System.ComponentModel.DataAnnotations;
namespace EventParkingReservationSystem.API.DTOs.Bookings;
public class CreateBookingDto
{
    [Range(1, int.MaxValue)] public int EventId { get; set; }
    [MinLength(1)] public List<int> SeatIds { get; set; } = new();
    public int? ParkingSlotId { get; set; }
}
public class AddSeatsDto
{
    [MinLength(1)] public List<int> SeatIds { get; set; } = new();
}
public record BookingSeatDto(int SeatId, string SeatNumber, decimal Price);
public record ParkingReservationDto(int ParkingSlotId, string SlotNumber, decimal Fee);
public record BookingResponseDto(
    int BookingId, string BookingNumber, int CustomerId, int EventId, string EventName,
    string Status, decimal TotalAmount, DateTime? HoldExpiresAtUtc,
    IReadOnlyList<BookingSeatDto> Seats, ParkingReservationDto? Parking,
    DateTime CreatedAtUtc);
public record HoldStatusDto(int BookingId, string Status, DateTime? HoldExpiresAtUtc, int RemainingSeconds, bool IsExpired);
