using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        Task<List<Booking>> GetAllAsync(
            int? showId = null,
            string? status = null);

        Task<Booking?> GetByIdAsync(int id);

        Task<List<Booking>> GetByShowIdAsync(int showId);

        Task<Show?> GetShowByIdAsync(int showId);

        Task<Booking> CreateWithSeatsAsync(
            Booking booking,
            List<int> seatIds);

        Task<Booking?> CancelWithSeatsAsync(int bookingId);
    }
}