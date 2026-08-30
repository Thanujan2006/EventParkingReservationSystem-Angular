using EventParkingReservationSystem.API.DTOs.Customers;
using EventParkingReservationSystem.API.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOs.Customers;
using WebApplication1.Exceptions;
using WebApplication1.Helpers;

namespace EventParkingReservationSystem.API.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _service;
    public CustomersController(ICustomerService service) => _service = service;

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<CustomerResponseDto>> Register(RegisterCustomerDto dto)
    {
        var result = await _service.RegisterAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = result.CustomerId }, result);
    }

    [Authorize(Roles = "Customer,Administrator")]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerResponseDto>> Get(int id)
    {
        if (User.Role() == "Customer" && User.UserId() != id)
            throw new ApiException(403, "You can only view your own profile.");
        return Ok(await _service.GetAsync(id));
    }

    //[Authorize(Roles = "Customer")]
    //[HttpPut("{id:int}")]
    //public async Task<ActionResult<CustomerResponseDto>> Update(int id, UpdateCustomerDto dto)
    //{
    //    if (User.UserId() != id)
    //        throw new ApiException(403, "You can only update your own profile.");
    //    return Ok(await _service.UpdateAsync(id, dto));
    //}

    [Authorize(Roles = "Administrator")]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CustomerResponseDto>>> Search([FromQuery] string? search)
        => Ok(await _service.SearchAsync(search));

    //[Authorize(Roles = "Administrator")]
    //[HttpDelete("{id:int}")]
    //public async Task<IActionResult> Deactivate(int id)
    //{
    //    await _service.DeactivateAsync(id);
    //    return Ok(new { message = "Customer deactivated." });
    //}

    [Authorize(Roles = "Administrator")]
    [HttpPost("{id:int}/reactivate")]
    public async Task<IActionResult> Reactivate(int id)
    {
        await _service.ReactivateAsync(id);
        return Ok(new { message = "Customer reactivated." });
    }
}

   

