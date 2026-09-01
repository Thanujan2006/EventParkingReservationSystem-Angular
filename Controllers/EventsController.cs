using EventParkingReservationSystem.API.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EventParkingReservationSystem.API.DTOs.Events;

namespace EventParkingReservationSystem.API.Controllers;

[ApiController]
[Authorize(Roles = "Customer,Administrator")]
[Route("api/events")]
public class EventsController : ControllerBase
{
    private readonly IEventService _service;
    public EventsController(IEventService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EventResponseDto>>> GetAll(
        [FromQuery] string? name,
        [FromQuery] DateOnly? date,
        [FromQuery] int? venueId,
        [FromQuery] int? categoryId)
        => Ok(await _service.GetAllAsync(name, date, venueId, categoryId));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EventResponseDto>> Get(int id)
        => Ok(await _service.GetAsync(id));

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<ActionResult<EventResponseDto>> Create(EventUpsertDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = result.EventId }, result);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<EventResponseDto>> Update(int id, EventUpsertDto dto)
        => Ok(await _service.UpdateAsync(id, dto));

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return Ok(new { message = "Event deleted." });
    }

}
