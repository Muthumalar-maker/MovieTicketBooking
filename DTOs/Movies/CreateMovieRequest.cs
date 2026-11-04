
namespace MovieTicketBooking.DTOs.Movies
{
    public class CreateMovieRequest
    {
        public string Title { get; set; } = "";

        public string? Description { get; set; }

        public int DurationMinutes { get; set; }

        public DateTime ReleaseDate { get; set; }

        public string Language { get; set; } = "";

        public string Genre { get; set; } = "";
    }
}
