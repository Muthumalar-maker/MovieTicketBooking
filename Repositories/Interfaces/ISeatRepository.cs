using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface ISeatRepository
    {
        Task<List<Seat>> GetAllAsync();
        Task<Seat?> GetByIdAsync(int id);
        Task<Seat> AddAsync(Seat seat);
        Task UpdateAsync(Seat seat);
        Task<bool> ScreenExistsAsync(int screenId);
        Task<bool> ExistsByPositionAsync(
            int screenId, string rowName, int seatNumber,
            int? excludeSeatId = null);
        Task<bool> HasReferencesAsync(int seatId);
        Task<bool> DeleteAsync(Seat seat);
    }
}