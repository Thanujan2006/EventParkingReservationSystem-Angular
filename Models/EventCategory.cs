using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class EventCategory
{
    public int EventCategoryId { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Event> Events { get; set; } = new List<Event>();

}
