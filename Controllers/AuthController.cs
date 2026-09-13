using EventParkingReservationSystem.API.DTOs.Auth;
using EventParkingReservationSystem.API.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace EventParkingReservationSystem.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;
    public AuthController(IAuthService service) => _service = service;

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
        => Ok(await _service.LoginCustomerAsync(dto));

    [AllowAnonymous]
    [HttpPost("admin-login")]
    public async Task<ActionResult<AuthResponseDto>> AdminLogin(LoginDto dto)
        => Ok(await _service.LoginAdminAsync(dto));

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
    {
        await _service.ForgotPasswordAsync(dto);
        return Ok(new { message = "If that email is registered, a password reset link has been sent." });
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        await _service.ResetPasswordAsync(dto);
        return Ok(new { message = "Password reset successfully." });
    }

    [AllowAnonymous]
    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        await _service.VerifyEmailAsync(token);
        return Ok(new { message = "Email verified successfully." });
    }

    [AllowAnonymous]
    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerification(ResendVerificationDto dto)
    {
        await _service.ResendVerificationAsync(dto);
        return Ok(new { message = "If the account exists and is unverified, a new verification email has been sent." });
    }
}


