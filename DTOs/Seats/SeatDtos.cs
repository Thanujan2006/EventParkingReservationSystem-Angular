using System.ComponentModel.DataAnnotations;
namespace WebApplication1.DTOs.Seats;
public class CreateSeatMapDto
{
    [Range(1, 100)] public int Rows { get; set; }
    [Range(1, 100)] public int Columns { get; set; }
    [MaxLength(50)] public string? SeatType { get; set; }
    [Range(0, double.MaxValue)] public decimal? SeatPrice { get; set; }
}
public class UpdateSeatDto
{
    [Required, MaxLength(20)] public string SeatNumber { get; set; } = string.Empty;
    [MaxLength(50)] public string? SeatType { get; set; }
    [Range(0, double.MaxValue)] public decimal? SeatPrice { get; set; }
}

public record SeatResponseDto(
    int SeatId, 
    string SeatNumber, 
    string? SeatType,
    decimal Price,
    string Status);