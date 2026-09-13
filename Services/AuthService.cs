using EventParkingReservationSystem.API.DTOs.Auth;
using EventParkingReservationSystem.API.IRepositories;
using EventParkingReservationSystem.API.IServices;
using Microsoft.EntityFrameworkCore;

using EventParkingReservationSystem.API.Enums;
using EventParkingReservationSystem.API.Exceptions;
using EventParkingReservationSystem.API.Helpers;
using EventParkingReservationSystem.API.Repositories;

namespace EventParkingReservationSystem.API.Services;

public class AuthService : IAuthService
{

    private readonly IUnitOfWork _uow;
    private readonly IPasswordService _passwords;
    private readonly ITokenService _tokens;
    private readonly IEmailService _email;
    private readonly IConfiguration _config;

    public AuthService(
        IUnitOfWork uow,
        IPasswordService passwords,
        ITokenService tokens,
        IEmailService email,
        IConfiguration config)
    {
        _uow = uow;
        _passwords = passwords;
        _tokens = tokens;
        _email = email;
        _config = config;
    }

    public async Task<AuthResponseDto> LoginCustomerAsync(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var customer = await _uow.Customers.Query()
            .FirstOrDefaultAsync(x => x.Email == email);

        if (customer is null || !_passwords.Verify(customer.PasswordHash, dto.Password))
            throw new ApiException(401, "Invalid email or password.");

        if (customer.Status == CustomerStatus.Deactivated)
            throw new ApiException(403, "Account is deactivated.");

        if (!customer.EmailVerified)
            throw new ApiException(403, "Please verify your email before logging in.");

        var jwt = _tokens.CreateJwt(customer.CustomerId, customer.Name, customer.Email, "Customer");
        return new AuthResponseDto(customer.CustomerId, customer.Name, customer.Email, "Customer", jwt);
    }

    public async Task<AuthResponseDto> LoginAdminAsync(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var admin = await _uow.AdminUsers.Query()
            .FirstOrDefaultAsync(x => x.Email == email);

        if (admin is null || !admin.IsActive || !_passwords.Verify(admin.PasswordHash, dto.Password))
            throw new ApiException(401, "Invalid administrator credentials.");

        var jwt = _tokens.CreateJwt(admin.AdminUserId, admin.Name, admin.Email, "Administrator");
        return new AuthResponseDto(admin.AdminUserId, admin.Name, admin.Email, "Administrator", jwt);
    }

    public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var customer = await _uow.Customers.Query()
            .FirstOrDefaultAsync(x => x.Email == email);

        // Always return success to prevent email enumeration.
        if (customer is null)
            return;

        var rawToken = _tokens.CreateSecureToken();
        customer.PasswordResetTokenHash = _tokens.HashToken(rawToken);
        customer.PasswordResetTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(
            _config.GetValue<int?>("TokenSettings:PasswordResetMinutes") ?? 45);
        customer.UpdatedAtUtc = DateTime.UtcNow;

        await _uow.SaveChangesAsync();

        var frontend = _config["Frontend:BaseUrl"] ?? "http://localhost:5500";
        var link = $"{frontend}/reset-password.html?token={Uri.EscapeDataString(rawToken)}";
        await _email.SendAsync(customer.Email, "Reset your password",
            $"Use this one-time link to reset your password:\n{link}\n\nThis link expires soon.");
    }

    public async Task ResetPasswordAsync(ResetPasswordDto dto)
    {
        var hash = _tokens.HashToken(dto.Token);
        var customer = await _uow.Customers.Query()
            .FirstOrDefaultAsync(x =>
                x.PasswordResetTokenHash == hash &&
                x.PasswordResetTokenExpiresAtUtc != null &&
                x.PasswordResetTokenExpiresAtUtc > DateTime.UtcNow);

        if (customer is null)
            throw new ApiException(400, "Invalid or expired password reset token.");

        customer.PasswordHash = _passwords.Hash(dto.NewPassword);
        customer.PasswordResetTokenHash = null;
        customer.PasswordResetTokenExpiresAtUtc = null;
        customer.UpdatedAtUtc = DateTime.UtcNow;

        await _uow.SaveChangesAsync();
    }

    public async Task VerifyEmailAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ApiException(400, "Verification token is required.");

        var hash = _tokens.HashToken(token);
        var customer = await _uow.Customers.Query()
            .FirstOrDefaultAsync(x =>
                x.EmailVerificationTokenHash == hash &&
                x.EmailVerificationTokenExpiresAtUtc != null &&
                x.EmailVerificationTokenExpiresAtUtc > DateTime.UtcNow);

        if (customer is null)
            throw new ApiException(400, "Invalid or expired verification token.");

        customer.EmailVerified = true;
        customer.EmailVerificationTokenHash = null;
        customer.EmailVerificationTokenExpiresAtUtc = null;
        customer.UpdatedAtUtc = DateTime.UtcNow;

        await _uow.SaveChangesAsync();
    }

    public async Task ResendVerificationAsync(ResendVerificationDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var customer = await _uow.Customers.Query()
            .FirstOrDefaultAsync(x => x.Email == email);

        // Generic response when account does not exist.
        if (customer is null || customer.EmailVerified)
            return;

        var rawToken = _tokens.CreateSecureToken();
        customer.EmailVerificationTokenHash = _tokens.HashToken(rawToken);
        customer.EmailVerificationTokenExpiresAtUtc = DateTime.UtcNow.AddHours(
            _config.GetValue<int?>("TokenSettings:EmailVerificationHours") ?? 24);
        customer.UpdatedAtUtc = DateTime.UtcNow;

        await _uow.SaveChangesAsync();

        var frontend = _config["Frontend:BaseUrl"] ?? "http://localhost:5500";
        var link = $"{frontend}/verify.html?token={Uri.EscapeDataString(rawToken)}";
        await _email.SendAsync(customer.Email, "Verify your email",
            $"Verify your Event & Parking Reservation account:\n{link}");
    }
}

