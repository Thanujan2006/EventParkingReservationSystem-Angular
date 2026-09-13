using EventParkingReservationSystem.API.DTOs.Auth;


namespace EventParkingReservationSystem.API.IServices;

public interface IAuthService
{
    Task<AuthResponseDto> LoginCustomerAsync(LoginDto dto);
    Task<AuthResponseDto> LoginAdminAsync(LoginDto dto);
    Task ForgotPasswordAsync(ForgotPasswordDto dto);
    Task ResetPasswordAsync(ResetPasswordDto dto);
    Task VerifyEmailAsync(string token);
    Task ResendVerificationAsync(ResendVerificationDto dto);
}
