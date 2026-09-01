
using EventParkingReservationSystem.API.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOs.Seats;

namespace EventParkingReservationSystem.API.Controllers;

[ApiController]
[Authorize(Roles = "Customer,Administrator")]
[Route("api/events/{eventId:int}/seats")]
public class SeatsController : ControllerBase
{
    private readonly ISeatService _service;
    public SeatsController(ISeatService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SeatResponseDto>>> GetMap(int eventId)
        => Ok(await _service.GetMapAsync(eventId));

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<ActionResult<IReadOnlyList<SeatResponseDto>>> CreateMap(int eventId, CreateSeatMapDto dto)
        => StatusCode(201, await _service.CreateMapAsync(eventId, dto));

    [Authorize(Roles = "Administrator")]
    [HttpPut("{seatId:int}")]
    public async Task<ActionResult<SeatResponseDto>> Update(int eventId, int seatId, UpdateSeatDto dto)
        => Ok(await _service.UpdateAsync(eventId, seatId, dto));

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{seatId:int}")]
    public async Task<IActionResult> Delete(int eventId, int seatId)
    {
        await _service.DeleteAsync(eventId, seatId);
        return Ok(new { message = "Seat deleted." });
    }

}
