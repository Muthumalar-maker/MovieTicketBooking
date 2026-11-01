namespace MovieTicketBooking.DTOs.Movies
{
    public class PagedMovieResponse
    {
        public List<MovieResponse> Items { get; set; } = new List<MovieResponse>();

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }
    }
}