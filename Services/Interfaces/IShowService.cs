using MovieTicketBooking.DTOs.Shows;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface IShowService
    {
        Task<List<ShowResponse>> GetAllAsync();
        Task<ShowResponse?> GetByIdAsync(int id);
        Task<ShowResponse> CreateAsync(CreateShowRequest request);
        Task<ShowResponse?> UpdateAsync(int id, UpdateShowRequest request);
        Task<bool> DeleteAsync(int id);
    }
}