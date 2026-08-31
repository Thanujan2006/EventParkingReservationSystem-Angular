using System.Security.Claims;

namespace EventParkingReservationSystem.API.Helpers;

public static class ClaimsPrincipalExtensions
{
    public static int UserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out var id) ? id : 0;
    }

    public static string Role(this ClaimsPrincipal user)
        => user.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
}
