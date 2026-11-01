using MovieTicketBooking.DTOs.Shows;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class ShowService : IShowService
    {
        private readonly IShowRepository _showRepository;

        public ShowService(IShowRepository showRepository)
        {
            _showRepository = showRepository;
        }

        public async Task<List<ShowResponse>> GetAllAsync()
        {
            var shows = await _showRepository.GetAllAsync();

            return shows.Select(MapToResponse).ToList();
        }

        public async Task<ShowResponse?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid show ID.");

            var show = await _showRepository.GetByIdAsync(id);

            return show == null ? null : MapToResponse(show);
        }

        public async Task<ShowResponse> CreateAsync(CreateShowRequest request)
        {
            ValidateRequest(request.MovieId, request.ScreenId,
                request.StartTime, request.TicketPrice);

            var duration = await _showRepository.GetMovieDurationAsync(request.MovieId);

            if (duration == null)
                throw new ArgumentException("Movie not found.");

            if (!await _showRepository.ScreenExistsAsync(request.ScreenId))
                throw new ArgumentException("Screen not found.");

            var endTime = request.StartTime.AddMinutes(duration.Value);

            if (await _showRepository.HasOverlapAsync(
                request.ScreenId, request.StartTime, endTime))
            {
                throw new ArgumentException(
                    "Another show is already scheduled during this time on the selected screen.");
            }

            var seatIds = await _showRepository.GetScreenSeatIdsAsync(request.ScreenId);

            if (seatIds.Count == 0)
                throw new ArgumentException(
                    "Cannot create a show because the selected screen has no seats.");

            var show = new Show
            {
                MovieId = request.MovieId,
                ScreenId = request.ScreenId,
                StartTime = request.StartTime,
                EndTime = endTime,
                TicketPrice = request.TicketPrice
            };

            var createdShow = await _showRepository.AddWithSeatsAsync(show, seatIds);

            return MapToResponse(createdShow);
        }

        public async Task<ShowResponse?> UpdateAsync(
            int id, UpdateShowRequest request)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid show ID.");

            ValidateRequest(request.MovieId, request.ScreenId,
                request.StartTime, request.TicketPrice);

            var show = await _showRepository.GetByIdAsync(id);

            if (show == null)
                return null;

            if (await _showRepository.HasBookingsAsync(id))
            {
                throw new InvalidOperationException(
                    "Cannot update a show that already has bookings.");
            }

            var duration = await _showRepository.GetMovieDurationAsync(request.MovieId);

            if (duration == null)
                throw new ArgumentException("Movie not found.");

            if (!await _showRepository.ScreenExistsAsync(request.ScreenId))
                throw new ArgumentException("Screen not found.");

            var endTime = request.StartTime.AddMinutes(duration.Value);

            if (await _showRepository.HasOverlapAsync(
                request.ScreenId, request.StartTime, endTime, id))
            {
                throw new ArgumentException(
                    "Another show is already scheduled during this time on the selected screen.");
            }

            var seatIds = await _showRepository.GetScreenSeatIdsAsync(request.ScreenId);

            if (seatIds.Count == 0)
                throw new ArgumentException("The selected screen has no seats.");

            show.MovieId = request.MovieId;
            show.ScreenId = request.ScreenId;
            show.StartTime = request.StartTime;
            show.EndTime = endTime;
            show.TicketPrice = request.TicketPrice;

            await _showRepository.UpdateWithSeatsAsync(show, seatIds);

            // Recreate the show-seat list when the screen changes.
            // Existing show-seat records are retained if the screen is unchanged.
            // The repository method below handles the screen-change case.

            return MapToResponse(show);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid show ID.");

            var show = await _showRepository.GetByIdAsync(id);

            if (show == null)
                return false;

            if (await _showRepository.HasBookingsAsync(id))
            {
                throw new InvalidOperationException(
                    "Cannot delete a show that already has bookings.");
            }

            return await _showRepository.DeleteAsync(show);
        }

        private static void ValidateRequest(
            int movieId, int screenId, DateTime startTime, decimal ticketPrice)
        {
            if (movieId <= 0)
                throw new ArgumentException("Movie ID must be greater than zero.");

            if (screenId <= 0)
                throw new ArgumentException("Screen ID must be greater than zero.");

            if (startTime == default)
                throw new ArgumentException("A valid show start time is required.");

            if (ticketPrice <= 0)
                throw new ArgumentException("Ticket price must be greater than zero.");
        }

        private static ShowResponse MapToResponse(Show show)
        {
            return new ShowResponse
            {
                ShowId = show.ShowId,
                MovieId = show.MovieId,
                ScreenId = show.ScreenId,
                StartTime = show.StartTime,
                EndTime = show.EndTime,
                TicketPrice = show.TicketPrice
            };
        }
    }
}