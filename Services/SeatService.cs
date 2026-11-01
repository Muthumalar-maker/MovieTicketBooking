using MovieTicketBooking.DTOs.Seats;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class SeatService : ISeatService
    {
        private readonly ISeatRepository _seatRepository;

        public SeatService(ISeatRepository seatRepository)
        {
            _seatRepository = seatRepository;
        }

        public async Task<List<SeatResponse>> GetAllAsync()
        {
            var seats = await _seatRepository.GetAllAsync();

            return seats.Select(MapToResponse).ToList();
        }

        public async Task<SeatResponse?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid seat ID.");

            var seat = await _seatRepository.GetByIdAsync(id);

            return seat == null ? null : MapToResponse(seat);
        }

        public async Task<SeatResponse> CreateAsync(CreateSeatRequest request)
        {
            ValidateSeat(request.ScreenId, request.RowName,
                request.SeatNumber, request.SeatType);

            if (!await _seatRepository.ScreenExistsAsync(request.ScreenId))
                throw new ArgumentException("Screen not found.");

            if (await _seatRepository.ExistsByPositionAsync(
                request.ScreenId, request.RowName.Trim(), request.SeatNumber))
                throw new ArgumentException("This seat position already exists.");

            var seat = new Seat
            {
                ScreenId = request.ScreenId,
                RowName = request.RowName.Trim(),
                SeatNumber = request.SeatNumber,
                SeatType = request.SeatType.Trim()
            };

            var createdSeat = await _seatRepository.AddAsync(seat);

            return MapToResponse(createdSeat);
        }

        public async Task<SeatResponse?> UpdateAsync(
            int id, UpdateSeatRequest request)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid seat ID.");

            ValidateSeat(request.ScreenId, request.RowName,
                request.SeatNumber, request.SeatType);

            var seat = await _seatRepository.GetByIdAsync(id);

            if (seat == null)
                return null;

            if (!await _seatRepository.ScreenExistsAsync(request.ScreenId))
                throw new ArgumentException("Screen not found.");

            if (await _seatRepository.ExistsByPositionAsync(
                request.ScreenId, request.RowName.Trim(),
                request.SeatNumber, id))
                throw new ArgumentException("This seat position already exists.");

            if (await _seatRepository.HasReferencesAsync(id) &&
                (seat.ScreenId != request.ScreenId ||
                 seat.RowName != request.RowName.Trim() ||
                 seat.SeatNumber != request.SeatNumber))
            {
                throw new InvalidOperationException(
                    "Cannot change the screen or position of a seat that is already used in shows or bookings.");
            }

            seat.ScreenId = request.ScreenId;
            seat.RowName = request.RowName.Trim();
            seat.SeatNumber = request.SeatNumber;
            seat.SeatType = request.SeatType.Trim();

            await _seatRepository.UpdateAsync(seat);

            return MapToResponse(seat);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid seat ID.");

            var seat = await _seatRepository.GetByIdAsync(id);

            if (seat == null)
                return false;

            if (await _seatRepository.HasReferencesAsync(id))
            {
                throw new InvalidOperationException(
                    "Cannot delete a seat that is already used in shows or bookings.");
            }

            return await _seatRepository.DeleteAsync(seat);
        }

        private static void ValidateSeat(
            int screenId, string rowName, int seatNumber, string seatType)
        {
            if (screenId <= 0)
                throw new ArgumentException("Screen ID must be greater than zero.");

            if (string.IsNullOrWhiteSpace(rowName) || rowName.Trim().Length > 5)
                throw new ArgumentException("Row name is required and must not exceed 5 characters.");

            if (seatNumber <= 0)
                throw new ArgumentException("Seat number must be greater than zero.");

            if (string.IsNullOrWhiteSpace(seatType) || seatType.Trim().Length > 30)
                throw new ArgumentException("Seat type is required and must not exceed 30 characters.");
        }

        private static SeatResponse MapToResponse(Seat seat)
        {
            return new SeatResponse
            {
                SeatId = seat.SeatId,
                ScreenId = seat.ScreenId,
                RowName = seat.RowName,
                SeatNumber = seat.SeatNumber,
                SeatType = seat.SeatType
            };
        }
    }
}