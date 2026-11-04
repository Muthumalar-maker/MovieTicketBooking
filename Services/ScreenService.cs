using MovieTicketBooking.DTOs.Screens;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class ScreenService : IScreenService
    {
        private readonly IScreenRepository _screenRepository;

        public ScreenService(IScreenRepository screenRepository)
        {
            _screenRepository = screenRepository;
        }

        public async Task<List<ScreenResponse>> GetAllAsync()
        {
            var screens = await _screenRepository.GetAllAsync();

            return screens.Select(MapToResponse).ToList();
        }

        public async Task<ScreenResponse?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid screen ID.");

            var screen = await _screenRepository.GetByIdAsync(id);

            return screen == null ? null : MapToResponse(screen);
        }

        public async Task<ScreenResponse> CreateAsync(CreateScreenRequest request)
        {
            ValidateScreen(request.TheatreId, request.ScreenName);

            if (!await _screenRepository.TheatreExistsAsync(request.TheatreId))
                throw new ArgumentException("Theatre not found.");

            if (await _screenRepository.ExistsByNameAsync(
                request.TheatreId, request.ScreenName.Trim()))
                throw new ArgumentException("Screen name already exists in this theatre.");

            var screen = new Screen
            {
                TheatreId = request.TheatreId,
                ScreenName = request.ScreenName.Trim()
            };

            var created = await _screenRepository.AddAsync(screen);

            return MapToResponse(created);
        }

        public async Task<ScreenResponse?> UpdateAsync(
            int id, UpdateScreenRequest request)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid screen ID.");

            ValidateScreen(request.TheatreId, request.ScreenName);

            var screen = await _screenRepository.GetByIdAsync(id);

            if (screen == null)
                return null;

            if (!await _screenRepository.TheatreExistsAsync(request.TheatreId))
                throw new ArgumentException("Theatre not found.");

            if (await _screenRepository.ExistsByNameAsync(
                request.TheatreId, request.ScreenName.Trim(), id))
                throw new ArgumentException("Screen name already exists in this theatre.");

            screen.TheatreId = request.TheatreId;
            screen.ScreenName = request.ScreenName.Trim();

            await _screenRepository.UpdateAsync(screen);

            return MapToResponse(screen);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid screen ID.");

            var screen = await _screenRepository.GetByIdAsync(id);

            if (screen == null)
                return false;

            if (await _screenRepository.HasSeatsAsync(id) ||
                await _screenRepository.HasShowsAsync(id))
            {
                throw new InvalidOperationException(
                    "Cannot delete screen because seats or shows are associated with it.");
            }

            return await _screenRepository.DeleteAsync(screen);
        }

        private static void ValidateScreen(int theatreId, string screenName)
        {
            if (theatreId <= 0)
                throw new ArgumentException("Valid TheatreId is required.");

            if (string.IsNullOrWhiteSpace(screenName))
                throw new ArgumentException("Screen name is required.");

            if (screenName.Trim().Length > 50)
                throw new ArgumentException("Screen name cannot exceed 50 characters.");
        }

        private static ScreenResponse MapToResponse(Screen screen)
        {
            return new ScreenResponse
            {
                ScreenId = screen.ScreenId,
                TheatreId = screen.TheatreId,
                ScreenName = screen.ScreenName
            };
        }
    }
}