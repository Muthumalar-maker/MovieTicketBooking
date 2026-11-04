using MovieTicketBooking.DTOs.Theatres;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class TheatreService : ITheatreService
    {
        private readonly ITheatreRepository _theatreRepository;

        public TheatreService(ITheatreRepository theatreRepository)
        {
            _theatreRepository = theatreRepository;
        }

        public async Task<List<TheatreResponse>> GetAllAsync()
        {
            var theatres = await _theatreRepository.GetAllAsync();

            return theatres.Select(t => MapToResponse(t)).ToList();
        }

        public async Task<TheatreResponse?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid theatre ID.");

            var theatre = await _theatreRepository.GetByIdAsync(id);

            if (theatre == null)
                return null;

            return MapToResponse(theatre);
        }

        public async Task<TheatreResponse> CreateAsync(
            CreateTheatreRequest request)
        {
            ValidateTheatre(request.Name, request.Location);

            var theatre = new Theatre
            {
                Name = request.Name.Trim(),
                Location = request.Location.Trim()
            };

            var createdTheatre =
                await _theatreRepository.AddAsync(theatre);

            return MapToResponse(createdTheatre);
        }

        public async Task<TheatreResponse?> UpdateAsync(
            int id,
            UpdateTheatreRequest request)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid theatre ID.");

            ValidateTheatre(request.Name, request.Location);

            var theatre = await _theatreRepository.GetByIdAsync(id);

            if (theatre == null)
                return null;

            theatre.Name = request.Name.Trim();
            theatre.Location = request.Location.Trim();

            // The entity is tracked by EF Core.
            // SaveChangesAsync persists the updates.
            await _theatreRepository.UpdateAsync(theatre);

            return MapToResponse(theatre);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid theatre ID.");

            var theatre = await _theatreRepository.GetByIdAsync(id);

            if (theatre == null)
                return false;

            bool hasScreens =
                await _theatreRepository.HasScreensAsync(id);

            if (hasScreens)
            {
                throw new InvalidOperationException(
                    "Cannot delete theatre because screens are associated with it.");
            }

            return await _theatreRepository.DeleteAsync(theatre);
        }

        private static void ValidateTheatre(
            string name,
            string location)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Theatre name is required.");

            if (name.Trim().Length > 150)
                throw new ArgumentException(
                    "Theatre name cannot exceed 150 characters.");

            if (string.IsNullOrWhiteSpace(location))
                throw new ArgumentException("Theatre location is required.");

            if (location.Trim().Length > 200)
                throw new ArgumentException(
                    "Theatre location cannot exceed 200 characters.");
        }

        private static TheatreResponse MapToResponse(Theatre theatre)
        {
            return new TheatreResponse
            {
                TheatreId = theatre.TheatreId,
                Name = theatre.Name,
                Location = theatre.Location
            };
        }
    }
}