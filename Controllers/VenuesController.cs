
using EventParkingReservationSystem.API.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EventParkingReservationSystem.API.DTOs.Venues;

namespace EventParkingReservationSystem.API.Controllers;

[ApiController]
[Authorize(Roles = "Customer,Administrator")]
[Route("api/venues")]
public class VenuesController : ControllerBase
{
    private readonly IVenueService _service;
    public VenuesController(IVenueService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<VenueResponseDto>>> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VenueResponseDto>> Get(int id)
        => Ok(await _service.GetAsync(id));

    //[HttpGet("available")]
    //public async Task<ActionResult<IReadOnlyList<VenueResponseDto>>> Available(
    //    [FromQuery] DateOnly date,
    //    [FromQuery] TimeOnly startTime,
    //    [FromQuery] TimeOnly endTime,
    //    [FromQuery] int? venueId)
    //    => Ok(await _service.GetAvailableAsync(date, startTime, endTime, venueId));

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<ActionResult<VenueResponseDto>> Create(VenueDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = result.VenueId }, result);
    }
}

//    [Authorize(Roles = "Administrator")]
//    [HttpPut("{id:int}")]
//    public async Task<ActionResult<VenueResponseDto>> Update(int id, VenueDto dto)
//        => Ok(await _service.UpdateAsync(id, dto));

//    [Authorize(Roles = "Administrator")]
//    [HttpDelete("{id:int}")]
//    public async Task<IActionResult> Delete(int id)
//    {
//        await _service.DeleteAsync(id);
//        return Ok(new { message = "Venue deleted." });
//    }
//}
