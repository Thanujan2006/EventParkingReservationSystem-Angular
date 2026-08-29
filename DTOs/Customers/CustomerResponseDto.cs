namespace EventParkingReservationSystem.API.DTOs.Customers;

public record CustomerResponseDto(
    int CustomerId,
    string Name,
    string Email,
    string Phone,
    string Status,
    bool EmailVerified,
        int BookingCount,
    DateTime CreatedAtUtc);
