
using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.Models;

public class Movie
{
    public int MovieId { get; set; }

    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Range(1, int.MaxValue)]
    public int DurationMinutes { get; set; }

    public DateOnly ReleaseDate { get; set; }

    [Required]
    [MaxLength(50)]
    public string Language { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Genre { get; set; } = string.Empty;

    // Navigation Property
    public ICollection<Show> Shows { get; set; }
        = new List<Show>();
}
