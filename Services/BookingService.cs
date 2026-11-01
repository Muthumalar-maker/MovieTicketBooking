using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        // GET ALL BOOKINGS
        public async Task<List<BookingResponse>> GetAllAsync(
            int? showId = null,
            string? status = null)
        {
            var bookings = await _bookingRepository.GetAllAsync(showId, status);

            return bookings.Select(MapToResponse).ToList();
        }

        // GET BOOKING BY ID
        public async Task<BookingResponse?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid booking ID.");

            var booking = await _bookingRepository.GetByIdAsync(id);

            if (booking == null)
                return null;

            return MapToResponse(booking);
        }

        // GET BOOKINGS BY SHOW ID
        public async Task<List<BookingResponse>> GetByShowIdAsync(int showId)
        {
            if (showId <= 0)
                throw new ArgumentException("Invalid show ID.");

            var show = await _bookingRepository.GetShowByIdAsync(showId);

            if (show == null)
                throw new KeyNotFoundException("Show not found.");

            var bookings = await _bookingRepository.GetByShowIdAsync(showId);

            return bookings.Select(MapToResponse).ToList();
        }

        // CREATE BOOKING
        public async Task<BookingResponse> CreateAsync(
            CreateBookingRequest request)
        {
            if (request == null)
                throw new ArgumentException("Booking request is required.");

            if (string.IsNullOrWhiteSpace(request.CustomerName))
                throw new ArgumentException("Customer name is required.");

            if (request.CustomerName.Length > 100)
                throw new ArgumentException("Customer name cannot exceed 100 characters.");

            if (string.IsNullOrWhiteSpace(request.CustomerEmail))
                throw new ArgumentException("Customer email is required.");

            if (request.CustomerEmail.Length > 150 ||
                !IsValidEmail(request.CustomerEmail))
            {
                throw new ArgumentException("Enter a valid email address.");
            }

            if (string.IsNullOrWhiteSpace(request.CustomerPhone))
                throw new ArgumentException("Customer phone is required.");

            if (request.CustomerPhone.Length > 15)
                throw new ArgumentException("Phone number cannot exceed 15 characters.");

            if (request.ShowId <= 0)
                throw new ArgumentException("Invalid show ID.");

            if (request.SeatIds == null || request.SeatIds.Count == 0)
                throw new ArgumentException("Select at least one seat.");

            if (request.SeatIds.Any(id => id <= 0))
                throw new ArgumentException("Invalid seat ID.");

            if (request.SeatIds.Distinct().Count() != request.SeatIds.Count)
                throw new ArgumentException("Duplicate seat IDs are not allowed.");

            var show = await _bookingRepository.GetShowByIdAsync(request.ShowId);

            if (show == null)
                throw new KeyNotFoundException("Show not found.");

            if (show.StartTime <= DateTime.Now)
                throw new InvalidOperationException(
                    "Bookings are not allowed for a show that has already started.");

            if (show.TicketPrice < 0)
                throw new InvalidOperationException("Invalid show ticket price.");

            var booking = new Booking
            {
                BookingReference = "MOV" +
                    Guid.NewGuid().ToString("N")[..12].ToUpper(),

                ShowId = request.ShowId,
                CustomerName = request.CustomerName.Trim(),
                CustomerEmail = request.CustomerEmail.Trim(),
                CustomerPhone = request.CustomerPhone.Trim(),
                BookingDate = DateTime.UtcNow,
                Status = "Confirmed",
                TotalAmount = 0
            };

            var createdBooking =
                await _bookingRepository.CreateWithSeatsAsync(
                    booking,
                    request.SeatIds);

            return MapToResponse(createdBooking);
        }

        // CANCEL BOOKING
        public async Task<BookingResponse?> CancelAsync(int bookingId)
        {
            if (bookingId <= 0)
                throw new ArgumentException("Invalid booking ID.");

            var booking = await _bookingRepository.GetByIdAsync(bookingId);

            if (booking == null)
                return null;

            if (booking.Status != "Confirmed")
                throw new InvalidOperationException(
                    "Only confirmed bookings can be cancelled.");

            var cancelledBooking =
                await _bookingRepository.CancelWithSeatsAsync(bookingId);

            if (cancelledBooking == null)
                return null;

            return MapToResponse(cancelledBooking);
        }

        // VALIDATE EMAIL
        private bool IsValidEmail(string email)
        {
            try
            {
                var address = new System.Net.Mail.MailAddress(email);

                return address.Address == email.Trim();
            }
            catch
            {
                return false;
            }
        }

        // CONVERT BOOKING ENTITY INTO RESPONSE DTO
        private BookingResponse MapToResponse(Booking booking)
        {
            return new BookingResponse
            {
                BookingId = booking.BookingId,
                BookingReference = booking.BookingReference,
                ShowId = booking.ShowId,
                CustomerName = booking.CustomerName,
                CustomerEmail = booking.CustomerEmail,
                CustomerPhone = booking.CustomerPhone,
                BookingDate = booking.BookingDate,
                TotalAmount = booking.TotalAmount,
                Status = booking.Status,

                Seats = booking.BookingSeats.Select(bs =>
                    new BookingSeatResponse
                    {
                        SeatId = bs.SeatId,
                        SeatLabel = bs.Seat.RowName + bs.Seat.SeatNumber,
                        SeatType = bs.Seat.SeatType,
                        Price = bs.Price
                    }).ToList()
            };
        }
    }
}