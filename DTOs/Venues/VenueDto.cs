using System.ComponentModel.DataAnnotations;
namespace WebApplication1.DTOs.Venues;
public class VenueDto
{
    [Required, MaxLength(150)] public string Name { get; set; } = string.Empty;
    [Required, MaxLength(300)] public string Address { get; set; } = string.Empty;
    [Range(1, int.MaxValue)] public int TotalCapacity { get; set; }
}
public record VenueResponseDto(int VenueId, string Name, string Address, int TotalCapacity);



