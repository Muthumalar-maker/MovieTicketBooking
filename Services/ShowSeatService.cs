using MovieTicketBooking.DTOs.ShowSeats;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class ShowSeatService : IShowSeatService
    {
        private readonly IShowSeatRepository _showSeatRepository;

        public ShowSeatService(IShowSeatRepository showSeatRepository)
        {
            _showSeatRepository = showSeatRepository;
        }

        public async Task<List<ShowSeatResponse>> GetAllAsync()
        {
            var showSeats = await _showSeatRepository.GetAllAsync();

            return showSeats.Select(MapToResponse).ToList();
        }

        public async Task<ShowSeatResponse?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid show seat ID.");

            var showSeat = await _showSeatRepository.GetByIdAsync(id);

            return showSeat == null ? null : MapToResponse(showSeat);
        }

        public async Task<List<ShowSeatResponse>> GetByShowIdAsync(int showId)
        {
            if (showId <= 0)
                throw new ArgumentException("Show ID must be greater than zero.");

            if (!await _showSeatRepository.ShowExistsAsync(showId))
                throw new KeyNotFoundException("Show not found.");

            var showSeats = await _showSeatRepository.GetByShowIdAsync(showId);

            return showSeats.Select(MapToResponse).ToList();
        }

        public async Task<ShowSeatResponse> CreateAsync(CreateShowSeatRequest request)
        {
            ValidateRequest(request.ShowId, request.SeatId);

            if (!await _showSeatRepository.ShowExistsAsync(request.ShowId))
                throw new ArgumentException("Show not found.");

            if (!await _showSeatRepository.SeatBelongsToScreenAsync(
                request.ShowId, request.SeatId))
            {
                throw new ArgumentException(
                    "The selected seat does not belong to the screen assigned to this show.");
            }

            if (await _showSeatRepository.ExistsAsync(
                request.ShowId, request.SeatId))
            {
                throw new InvalidOperationException(
                    "This seat is already assigned to the selected show.");
            }

            var showSeat = new ShowSeat
            {
                ShowId = request.ShowId,
                SeatId = request.SeatId,
                Status = "Available"
            };

            var createdShowSeat = await _showSeatRepository.AddAsync(showSeat);

            var result = await _showSeatRepository.GetByIdAsync(
                createdShowSeat.ShowSeatId);

            return MapToResponse(result!);
        }

        public async Task<ShowSeatResponse?> UpdateAsync(
            int id, UpdateShowSeatRequest request)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid show seat ID.");

            ValidateRequest(request.ShowId, request.SeatId);

            var showSeat = await _showSeatRepository.GetByIdAsync(id);

            if (showSeat == null)
                return null;

            if (showSeat.Status != "Available")
            {
                throw new InvalidOperationException(
                    "Only available show seats can be updated.");
            }

            if (await _showSeatRepository.HasBookingReferencesAsync(id))
            {
                throw new InvalidOperationException(
                    "Cannot update a show seat that has booking history.");
            }

            if (!await _showSeatRepository.ShowExistsAsync(request.ShowId))
                throw new ArgumentException("Show not found.");

            if (!await _showSeatRepository.SeatBelongsToScreenAsync(
                request.ShowId, request.SeatId))
            {
                throw new ArgumentException(
                    "The selected seat does not belong to the screen assigned to this show.");
            }

            if (await _showSeatRepository.ExistsAsync(
                request.ShowId, request.SeatId, id))
            {
                throw new InvalidOperationException(
                    "This seat is already assigned to the selected show.");
            }

            showSeat.ShowId = request.ShowId;
            showSeat.SeatId = request.SeatId;

            await _showSeatRepository.UpdateAsync(showSeat);

            var updatedShowSeat = await _showSeatRepository.GetByIdAsync(id);

            return MapToResponse(updatedShowSeat!);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid show seat ID.");

            var showSeat = await _showSeatRepository.GetByIdAsync(id);

            if (showSeat == null)
                return false;

            if (showSeat.Status != "Available")
            {
                throw new InvalidOperationException(
                    "Only available show seats can be deleted.");
            }

            if (await _showSeatRepository.HasBookingReferencesAsync(id))
            {
                throw new InvalidOperationException(
                    "Cannot delete a show seat that has booking history.");
            }

            return await _showSeatRepository.DeleteAsync(showSeat);
        }

        private static void ValidateRequest(int showId, int seatId)
        {
            if (showId <= 0)
                throw new ArgumentException("Show ID must be greater than zero.");

            if (seatId <= 0)
                throw new ArgumentException("Seat ID must be greater than zero.");
        }

        private static ShowSeatResponse MapToResponse(ShowSeat showSeat)
        {
            return new ShowSeatResponse
            {
                ShowSeatId = showSeat.ShowSeatId,
                ShowId = showSeat.ShowId,
                SeatId = showSeat.SeatId,
                RowName = showSeat.Seat.RowName,
                SeatNumber = showSeat.Seat.SeatNumber,
                SeatType = showSeat.Seat.SeatType,
                Status = showSeat.Status
            };
        }
    }
}