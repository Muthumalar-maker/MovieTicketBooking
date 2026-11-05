
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
            string? status = null,
            int? userId = null)
        {
            var bookings = await _bookingRepository.GetAllAsync(
                showId, status, userId);

            return bookings.Select(MapToResponse).ToList();
        }

        // GET BOOKING BY ID
        public async Task<BookingResponse?> GetByIdAsync(
            int id,
            int? userId = null)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid booking ID.");

            var booking = await _bookingRepository.GetByIdAsync(id, userId);

            if (booking == null)
                return null;

            return MapToResponse(booking);
        }

        // GET BOOKINGS BY SHOW ID
        public async Task<List<BookingResponse>> GetByShowIdAsync(
            int showId,
            int? userId = null)
        {
            if (showId <= 0)
                throw new ArgumentException("Invalid show ID.");

            var show = await _bookingRepository.GetShowByIdAsync(showId);

            if (show == null)
                throw new KeyNotFoundException("Show not found.");

            var bookings = await _bookingRepository.GetByShowIdAsync(
                showId, userId);

            return bookings.Select(MapToResponse).ToList();
        }

        // CREATE BOOKING
        public async Task<BookingResponse> CreateAsync(
            CreateBookingRequest request,
            int userId,
            string customerName,
            string customerEmail)
        {
            if (request == null)
                throw new ArgumentException("Booking request is required.");

            if (userId <= 0)
                throw new ArgumentException("Invalid user ID.");

            if (string.IsNullOrWhiteSpace(customerName))
                throw new ArgumentException("Customer name is required.");

            if (customerName.Length > 100)
                throw new ArgumentException(
                    "Customer name cannot exceed 100 characters.");

            if (string.IsNullOrWhiteSpace(customerEmail) ||
                customerEmail.Length > 150 ||
                !IsValidEmail(customerEmail))
            {
                throw new ArgumentException(
                    "A valid customer email is required.");
            }

            if (string.IsNullOrWhiteSpace(request.CustomerPhone))
                throw new ArgumentException("Customer phone is required.");

            if (request.CustomerPhone.Length > 15)
                throw new ArgumentException(
                    "Phone number cannot exceed 15 characters.");

            if (request.ShowId <= 0)
                throw new ArgumentException("Invalid show ID.");

            if (request.SeatIds == null || request.SeatIds.Count == 0)
                throw new ArgumentException("Select at least one seat.");

            if (request.SeatIds.Any(id => id <= 0))
                throw new ArgumentException("Invalid seat ID.");

            if (request.SeatIds.Distinct().Count() != request.SeatIds.Count)
                throw new ArgumentException(
                    "Duplicate seat IDs are not allowed.");

            var show = await _bookingRepository.GetShowByIdAsync(
                request.ShowId);

            if (show == null)
                throw new KeyNotFoundException("Show not found.");

            if (show.StartTime <= DateTime.Now)
                throw new InvalidOperationException(
                    "Bookings are not allowed for a show that has already started.");

            if (show.TicketPrice < 0)
                throw new InvalidOperationException(
                    "Invalid show ticket price.");

            var booking = new Booking
            {
                BookingReference = "MOV" +
                    Guid.NewGuid().ToString("N")[..12].ToUpper(),

                UserId = userId,
                CustomerName = customerName.Trim(),
                CustomerEmail = customerEmail.Trim(),
                CustomerPhone = request.CustomerPhone.Trim(),
                ShowId = request.ShowId,
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
        public async Task<BookingResponse?> CancelAsync(
            int bookingId,
            int? userId = null)
        {
            if (bookingId <= 0)
                throw new ArgumentException("Invalid booking ID.");

            var booking = await _bookingRepository.GetByIdAsync(
                bookingId, userId);

            if (booking == null)
                return null;

            if (booking.Status != "Confirmed")
                throw new InvalidOperationException(
                    "Only confirmed bookings can be cancelled.");

            var cancelledBooking =
                await _bookingRepository.CancelWithSeatsAsync(
                    bookingId, userId);

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
