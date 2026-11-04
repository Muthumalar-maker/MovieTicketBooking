using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IShowRepository
    {
        Task<List<Show>> GetAllAsync();
        Task<Show?> GetByIdAsync(int id);
        Task<int?> GetMovieDurationAsync(int movieId);
        Task<bool> ScreenExistsAsync(int screenId);
        Task<List<int>> GetScreenSeatIdsAsync(int screenId);
        Task UpdateWithSeatsAsync(Show show, List<int> seatIds);
        Task<bool> HasOverlapAsync(
            int screenId, DateTime startTime, DateTime endTime,
            int? excludeShowId = null);
        Task<Show> AddWithSeatsAsync(Show show, List<int> seatIds);
        Task UpdateAsync(Show show);
        Task<bool> HasBookingsAsync(int showId);
        Task<bool> DeleteAsync(Show show);
    }
}