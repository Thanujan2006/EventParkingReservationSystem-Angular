using Microsoft.AspNetCore.Identity;

namespace EventParkingReservationSystem.API.Helpers;

public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<object> _hasher = new();

    public string Hash(string password)
        => _hasher.HashPassword(new object(), password);

    public bool Verify(string hash, string password)
        => _hasher.VerifyHashedPassword(new object(), hash, password) != PasswordVerificationResult.Failed;
}
