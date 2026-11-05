using MovieTicketBooking.DTOs.ShowSeats;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface IShowSeatService
    {
        Task<List<ShowSeatResponse>> GetAllAsync();
        Task<ShowSeatResponse?> GetByIdAsync(int id);
        Task<List<ShowSeatResponse>> GetByShowIdAsync(int showId);
        Task<ShowSeatResponse> CreateAsync(CreateShowSeatRequest request);
        Task<ShowSeatResponse?> UpdateAsync(int id, UpdateShowSeatRequest request);
        Task<bool> DeleteAsync(int id);
    }
}