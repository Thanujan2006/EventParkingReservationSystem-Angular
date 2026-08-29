using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class Venue
{
    public int VenueId { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(300)]
    public string Address { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int TotalCapacity { get; set; }

    public ICollection<Event> Events { get; set; } = new List<Event>();
}
