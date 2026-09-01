using EventParkingReservationSystem.API.DTOs.Dashboard;
using EventParkingReservationSystem.API.Helpers;
using EventParkingReservationSystem.API.IServices;
using EventParkingReservationSystem.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservationSystem.API.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;
    public DashboardController(IDashboardService service) => _service = service;

    [Authorize(Roles = "Customer")]
    [HttpGet("customer")]
    public async Task<ActionResult<CustomerDashboardDto>> Customer()
        => Ok(await _service.GetCustomerAsync(User.UserId()));

    [Authorize(Roles = "Administrator")]
    [HttpGet("admin")]
    public async Task<ActionResult<AdminDashboardDto>> Admin()
        => Ok(await _service.GetAdminAsync());
}
