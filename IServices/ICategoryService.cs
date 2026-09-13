using EventParkingReservationSystem.API.DTOs.Categories;

namespace EventParkingReservationSystem.API.IServices;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryResponseDto>> GetAllAsync();
    Task<CategoryResponseDto> CreateAsync(CategoryDto dto);
    Task<CategoryResponseDto> UpdateAsync(int id, CategoryDto dto);
    Task DeleteAsync(int id);
}


