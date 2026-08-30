using EventParkingReservationSystem.API.IRepositories;
using EventParkingReservationSystem.API.IServices;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DTOs.Categories;
using WebApplication1.Exceptions;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _uow;
    public CategoryService(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<CategoryResponseDto>> GetAllAsync()
        => await _uow.EventCategories.Query().AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new CategoryResponseDto(x.EventCategoryId, x.Name))
            .ToListAsync();

    public async Task<CategoryResponseDto> CreateAsync(CategoryDto dto)
    {
        var name = dto.Name.Trim();
        if (await _uow.EventCategories.Query().AnyAsync(x => x.Name == name))
            throw new ApiException(409, "Category already exists.");

        var category = new EventCategory { Name = name };
        await _uow.EventCategories.AddAsync(category);
        await _uow.SaveChangesAsync();
        return new(category.EventCategoryId, category.Name);
    }

    public async Task<CategoryResponseDto> UpdateAsync(int id, CategoryDto dto)
    {
        var category = await _uow.EventCategories.GetByIdAsync(id)
            ?? throw new ApiException(404, "Category not found.");

        var name = dto.Name.Trim();
        if (await _uow.EventCategories.Query().AnyAsync(x => x.Name == name && x.EventCategoryId != id))
            throw new ApiException(409, "Category already exists.");

        category.Name = name;
        await _uow.SaveChangesAsync();
        return new(category.EventCategoryId, category.Name);
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _uow.EventCategories.GetByIdAsync(id)
            ?? throw new ApiException(404, "Category not found.");

        //if (await _uow.Events.Query().AnyAsync(x => x.EventCategoryId == id))
        //    throw new ApiException(409, "This category is assigned to existing events.");

        _uow.EventCategories.Remove(category);
        await _uow.SaveChangesAsync();
    }
}
   

