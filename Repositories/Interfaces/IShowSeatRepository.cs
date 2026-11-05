using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IShowSeatRepository
    {
        Task<List<ShowSeat>> GetAllAsync();
        Task<ShowSeat?> GetByIdAsync(int id);
        Task<List<ShowSeat>> GetByShowIdAsync(int showId);
        Task<ShowSeat> AddAsync(ShowSeat showSeat);
        Task UpdateAsync(ShowSeat showSeat);
        Task<bool> ShowExistsAsync(int showId);
        Task<bool> SeatBelongsToScreenAsync(int showId, int seatId);
        Task<bool> ExistsAsync(int showId, int seatId, int? excludeId = null);
        Task<bool> HasBookingReferencesAsync(int showSeatId);
        Task<bool> DeleteAsync(ShowSeat showSeat);
    }
}