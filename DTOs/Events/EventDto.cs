using System.ComponentModel.DataAnnotations;
namespace WebApplication1.DTOs.Events;
public class EventUpsertDto
{
    [Required, MaxLength(200)] public string Name { get; set; } = string.Empty;
    [Range(1, int.MaxValue)] public int VenueId { get; set; }
    [Range(1, int.MaxValue)] public int EventCategoryId { get; set; }
    public DateOnly EventDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    [Range(0, double.MaxValue)] public decimal TicketPrice { get; set; }
    [Range(1, int.MaxValue)] public int Capacity { get; set; }
    [Range(0, double.MaxValue)] public decimal ParkingFee { get; set; }
}
public record EventResponseDto(
    int EventId, string Name, int VenueId, string VenueName,
    int EventCategoryId, string CategoryName,
    DateOnly EventDate, TimeOnly StartTime, TimeOnly EndTime,
    decimal TicketPrice, decimal ParkingFee, int Capacity);
