using EventParkingReservationSystem.API.DTOs.Bookings;
using EventParkingReservationSystem.API.DTOs.Parking;
using EventParkingReservationSystem.API.Exceptions;
using EventParkingReservationSystem.API.Helpers;
using EventParkingReservationSystem.API.IServices;
using EventParkingReservationSystem.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EventParkingReservationSystem.API.DTOs.Parking;

namespace EventParkingReservationSystem.API.Controllers;

[ApiController]
[Authorize]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _service;
    public BookingsController(IBookingService service) => _service = service;

    [Authorize(Roles = "Customer")]
    [HttpPost]
    public async Task<ActionResult<BookingResponseDto>> Create(CreateBookingDto dto)
        => StatusCode(201, await _service.CreateAsync(User.UserId(), dto));

    [Authorize(Roles = "Customer,Administrator")]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookingResponseDto>> Get(int id)
    {
        var item = await _service.GetAsync(id);
        EnsureOwnerOrAdmin(item.CustomerId);
        return Ok(item);
    }

    [Authorize(Roles = "Customer,Administrator")]
    [HttpGet("{id:int}/hold-status")]
    public async Task<ActionResult<HoldStatusDto>> HoldStatus(int id)
    {
        var item = await _service.GetAsync(id);
        EnsureOwnerOrAdmin(item.CustomerId);
        return Ok(await _service.GetHoldStatusAsync(id));
    }

    [Authorize(Roles = "Customer")]
    [HttpGet("customer/{customerId:int}")]
    public async Task<ActionResult<IReadOnlyList<BookingResponseDto>>> CustomerHistory(int customerId)
    {
        if (User.UserId() != customerId)
            throw new ApiException(403, "You can only view your own booking history.");
        return Ok(await _service.GetCustomerHistoryAsync(customerId));
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BookingResponseDto>>> ByEvent([FromQuery] int eventId)
        => Ok(await _service.GetByEventAsync(eventId));

    [Authorize(Roles = "Customer")]
    [HttpPost("{id:int}/seats")]
    public async Task<ActionResult<BookingResponseDto>> AddSeats(int id, AddSeatsDto dto)
    {
        var item = await _service.GetAsync(id);
        if (item.CustomerId != User.UserId())
            throw new ApiException(403, "You can only modify your own booking.");
        return Ok(await _service.AddSeatsAsync(id, dto.SeatIds));
    }

    [Authorize(Roles = "Customer")]
    [HttpPost("{id:int}/parking")]
    public async Task<ActionResult<BookingResponseDto>> AddParking(int id, AddParkingDto dto)
    {
        var item = await _service.GetAsync(id);
        if (item.CustomerId != User.UserId())
            throw new ApiException(403, "You can only modify your own booking.");
        return Ok(await _service.AddParkingAsync(id, dto.ParkingSlotId));
    }

    [Authorize(Roles = "Customer")]
    [HttpDelete("{id:int}/parking")]
    public async Task<ActionResult<BookingResponseDto>> RemoveParking(int id)
    {
        var item = await _service.GetAsync(id);
        if (item.CustomerId != User.UserId())
            throw new ApiException(403, "You can only modify your own booking.");
        return Ok(await _service.RemoveParkingAsync(id));
    }

    [Authorize(Roles = "Customer,Administrator")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Cancel(int id)
    {
        var item = await _service.GetAsync(id);
        EnsureOwnerOrAdmin(item.CustomerId);
        await _service.CancelAsync(id);
        return Ok(new { message = "Booking cancelled and held/reserved resources released." });
    }

    private void EnsureOwnerOrAdmin(int customerId)
    {
        if (User.Role() != "Administrator" && User.UserId() != customerId)
            throw new ApiException(403, "You are not authorized to access this booking.");
    }
}
