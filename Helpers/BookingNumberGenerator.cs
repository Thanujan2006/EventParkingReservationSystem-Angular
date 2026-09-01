namespace EventParkingReservationSystem.API.Helpers;

public static class BookingNumberGenerator
{
    public static string NewNumber()
       => $"BKG-{DateTime.UtcNow:yyyy}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";

}
