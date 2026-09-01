using EventParkingReservationSystem.API.DTOs.Notifications;
using EventParkingReservationSystem.API.Helpers;
using EventParkingReservationSystem.API.IServices;
using EventParkingReservationSystem.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservationSystem.API.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _service;
    public NotificationsController(INotificationService service) => _service = service;

    // Present in Swagger for the BRD endpoint, but deliberately unavailable
    // to Customer/Administrator tokens. Notifications are created internally.
    [Authorize(Roles = "InternalSystem")]
    [HttpPost]
    public IActionResult InternalOnly()
        => StatusCode(403, new { message = "This endpoint is for internal use only." });

    [Authorize(Roles = "Customer")]
    [HttpGet("customer/{customerId:int}")]
    public async Task<ActionResult<IReadOnlyList<NotificationResponseDto>>> GetCustomer(int customerId)
    {
        if (User.UserId() != customerId)
            return Forbid();
        return Ok(await _service.GetForCustomerAsync(customerId));
    }

    [Authorize(Roles = "Customer")]
    [HttpPut("{id:int}/read")]
    public async Task<IActionResult> MarkRead(int id)
    {
        await _service.MarkReadAsync(User.UserId(), id);
        return Ok(new { message = "Notification marked as read." });
    }
}
