
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        Task<List<Booking>> GetAllAsync(
            int? showId = null,
            string? status = null,
            int? userId = null);

        Task<Booking?> GetByIdAsync(
            int id,
            int? userId = null);

        Task<Booking?> GetByIdForUpdateAsync(int id);

        Task<List<Booking>> GetByShowIdAsync(
            int showId,
            int? userId = null);

        Task<Show?> GetShowByIdAsync(int showId);

        Task<Booking> CreateWithSeatsAsync(
            Booking booking,
            List<int> seatIds);

        Task<Booking?> CancelWithSeatsAsync(
            int bookingId,
            int? userId = null);
    }
}
