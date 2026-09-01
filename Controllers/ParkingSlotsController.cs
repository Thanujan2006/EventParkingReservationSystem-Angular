
using EventParkingReservationSystem.API.DTOs.Parking;
using EventParkingReservationSystem.API.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EventParkingReservationSystem.API.DTOs.Parking;

namespace EventParkingReservationSystem.API.Controllers;

[ApiController]
[Authorize(Roles = "Customer,Administrator")]
[Route("api/events/{eventId:int}/parking-slots")]
public class ParkingSlotsController : ControllerBase
{
    private readonly IParkingService _service;
    public ParkingSlotsController(IParkingService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ParkingSlotResponseDto>>> GetLayout(int eventId)
        => Ok(await _service.GetLayoutAsync(eventId));

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<ActionResult<IReadOnlyList<ParkingSlotResponseDto>>> CreateLayout(
        int eventId, CreateParkingLayoutDto dto)
        => StatusCode(201, await _service.CreateLayoutAsync(eventId, dto));

    [Authorize(Roles = "Administrator")]
    [HttpPut("{slotId:int}")]
    public async Task<ActionResult<ParkingSlotResponseDto>> Update(
        int eventId, int slotId, UpdateParkingSlotDto dto)
        => Ok(await _service.UpdateAsync(eventId, slotId, dto));

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{slotId:int}")]
    public async Task<IActionResult> Delete(int eventId, int slotId)
    {
        await _service.DeleteAsync(eventId, slotId);
        return Ok(new { message = "Parking slot deleted." });
    }

}
