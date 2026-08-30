using System.ComponentModel.DataAnnotations;
namespace WebApplication1.DTOs.Categories;
public class CategoryDto
{
    [Required, MaxLength(100)] public string Name { get; set; } = string.Empty;
}
public record CategoryResponseDto(int EventCategoryId, string Name);



