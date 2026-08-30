using EventParkingReservationSystem.API.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOs.Categories;

namespace EventParkingReservationSystem.API.Controllers;

[ApiController]
[Authorize(Roles = "Customer,Administrator")]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _service;
    public CategoriesController(ICategoryService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryResponseDto>>> GetAll()
        => Ok(await _service.GetAllAsync());

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<ActionResult<CategoryResponseDto>> Create(CategoryDto dto)
        => StatusCode(201, await _service.CreateAsync(dto));

    [Authorize(Roles = "Administrator")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoryResponseDto>> Update(int id, CategoryDto dto)
        => Ok(await _service.UpdateAsync(id, dto));

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return Ok(new { message = "Category deleted." });
    }
}


