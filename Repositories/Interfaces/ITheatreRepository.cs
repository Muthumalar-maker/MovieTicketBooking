using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface ITheatreRepository
    {
        Task<List<Theatre>> GetAllAsync();

        Task<Theatre?> GetByIdAsync(int id);

        Task<Theatre> AddAsync(Theatre theatre);

        Task UpdateAsync(Theatre theatre);

        Task<bool> HasScreensAsync(int theatreId);

        Task<bool> DeleteAsync(Theatre theatre);
    }
}