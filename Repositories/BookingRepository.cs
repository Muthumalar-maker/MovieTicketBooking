using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET ALL BOOKINGS
        public async Task<List<Booking>> GetAllAsync(
            int? showId = null,
            string? status = null)
        {
            var query = _context.Bookings
                .Include(b => b.Show)
                .Include(b => b.BookingSeats)
                    .ThenInclude(bs => bs.Seat)
                .AsQueryable();

            if (showId.HasValue)
            {
                query = query.Where(b => b.ShowId == showId.Value);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(b => b.Status == status);
            }

            return await query
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
        }

        // GET BOOKING BY ID
        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Show)
                .Include(b => b.BookingSeats)
                    .ThenInclude(bs => bs.Seat)
                .FirstOrDefaultAsync(b => b.BookingId == id);
        }

        // GET BOOKINGS BY SHOW ID
        public async Task<List<Booking>> GetByShowIdAsync(int showId)
        {
            return await _context.Bookings
                .Include(b => b.Show)
                .Include(b => b.BookingSeats)
                    .ThenInclude(bs => bs.Seat)
                .Where(b => b.ShowId == showId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
        }

        // CHECK AND GET SHOW
        public async Task<Show?> GetShowByIdAsync(int showId)
        {
            return await _context.Shows
                .FirstOrDefaultAsync(s => s.ShowId == showId);
        }

        // CREATE BOOKING WITH SEATS
        public async Task<Booking> CreateWithSeatsAsync(
            Booking booking,
            List<int> seatIds)
        {
            if (seatIds.Count == 0 ||
                seatIds.Distinct().Count() != seatIds.Count)
            {
                throw new InvalidOperationException(
                    "Select at least one seat without duplicates.");
            }

            using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var show = await _context.Shows
                    .FirstOrDefaultAsync(s => s.ShowId == booking.ShowId);

                if (show == null)
                {
                    throw new KeyNotFoundException("Show not found.");
                }

                var showSeats = await _context.ShowSeats
                    .Include(ss => ss.Seat)
                    .Where(ss =>
                        ss.ShowId == booking.ShowId &&
                        seatIds.Contains(ss.SeatId))
                    .ToListAsync();

                if (showSeats.Count != seatIds.Count)
                {
                    throw new InvalidOperationException(
                        "One or more selected seats do not belong to this show.");
                }

                if (showSeats.Any(ss => ss.Status != "Available"))
                {
                    throw new InvalidOperationException(
                        "One or more selected seats are already booked.");
                }

                booking.TotalAmount =
                    show.TicketPrice * showSeats.Count;

                booking.Show = show;

                foreach (var showSeat in showSeats)
                {
                    showSeat.Status = "Booked";

                    booking.BookingSeats.Add(new BookingSeat
                    {
                        SeatId = showSeat.SeatId,
                        Seat = showSeat.Seat,
                        Price = show.TicketPrice
                    });
                }

                _context.Bookings.Add(booking);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return booking;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // CANCEL BOOKING AND RELEASE SEATS
        public async Task<Booking?> CancelWithSeatsAsync(int bookingId)
        {
            using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Show)
                    .Include(b => b.BookingSeats)
                        .ThenInclude(bs => bs.Seat)
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);

                if (booking == null)
                {
                    await transaction.RollbackAsync();
                    return null;
                }

                if (booking.Status != "Confirmed")
                {
                    throw new InvalidOperationException(
                        "Only confirmed bookings can be cancelled.");
                }

                var seatIds = booking.BookingSeats
                    .Select(bs => bs.SeatId)
                    .ToList();

                var showSeats = await _context.ShowSeats
                    .Where(ss =>
                        ss.ShowId == booking.ShowId &&
                        seatIds.Contains(ss.SeatId))
                    .ToListAsync();

                if (showSeats.Count != seatIds.Count)
                {
                    throw new InvalidOperationException(
                        "Booking seat records could not be matched.");
                }

                booking.Status = "Cancelled";

                foreach (var showSeat in showSeats)
                {
                    showSeat.Status = "Available";
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return booking;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}