using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IScreenRepository
    {
        Task<List<Screen>> GetAllAsync();

        Task<Screen?> GetByIdAsync(int id);

        Task<Screen> AddAsync(Screen screen);

        Task UpdateAsync(Screen screen);

        Task<bool> ExistsByNameAsync(int theatreId, string screenName, int? excludeScreenId = null);

        Task<bool> HasSeatsAsync(int screenId);

        Task<bool> HasShowsAsync(int screenId);

        Task<bool> DeleteAsync(Screen screen);

        Task<bool> TheatreExistsAsync(int theatreId);
    }
}