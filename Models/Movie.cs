
namespace MovieTicketBooking.Models
{
    public class Movie
    {
        public int MovieId { get; set; }

        public string Title { get; set; } = "";

        public string? Description { get; set; }

        public int DurationMinutes { get; set; }

        public DateTime ReleaseDate { get; set; }

        public string Language { get; set; } = "";

        public string Genre { get; set; } = "";

        // Navigation Property
        public List<Show> Shows { get; set; } = new List<Show>();
    }
}
