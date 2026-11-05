
using MovieTicketBooking.DTOs.Bookings;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface IBookingService
    {
        Task<List<BookingResponse>> GetAllAsync(
            int? showId = null,
            string? status = null,
            int? userId = null);

        Task<BookingResponse?> GetByIdAsync(
            int id,
            int? userId = null);

        Task<List<BookingResponse>> GetByShowIdAsync(
            int showId,
            int? userId = null);

        Task<BookingResponse> CreateAsync(
            CreateBookingRequest request,
            int userId,
            string customerName,
            string customerEmail);

        Task<BookingResponse?> CancelAsync(
            int bookingId,
            int? userId = null);
    }
}
