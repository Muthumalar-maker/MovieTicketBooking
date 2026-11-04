using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IMovieRepository
    {
        Task<(List<Movie> Movies, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? title);

        Task<Movie?> GetByIdAsync(int id);

        Task<List<Movie>> SearchByTitleAsync(string title);

        Task<Movie> AddAsync(Movie movie);

        Task UpdateAsync(Movie movie);

        Task<bool> HasShowsAsync(int movieId);

        Task DeleteAsync(Movie movie);
    }
}