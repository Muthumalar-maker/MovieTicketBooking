using MovieTicketBooking.DTOs.Seats;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface ISeatService
    {
        Task<List<SeatResponse>> GetAllAsync();
        Task<SeatResponse?> GetByIdAsync(int id);
        Task<SeatResponse> CreateAsync(CreateSeatRequest request);
        Task<SeatResponse?> UpdateAsync(int id, UpdateSeatRequest request);
        Task<bool> DeleteAsync(int id);
    }
}