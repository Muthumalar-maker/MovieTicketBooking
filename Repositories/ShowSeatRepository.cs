using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    public class ShowSeatRepository : IShowSeatRepository
    {
        private readonly ApplicationDbContext _context;

        public ShowSeatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ShowSeat>> GetAllAsync()
        {
            return await _context.ShowSeats
                .AsNoTracking()
                .Include(ss => ss.Seat)
                .OrderBy(ss => ss.ShowId)
                .ThenBy(ss => ss.Seat.RowName)
                .ThenBy(ss => ss.Seat.SeatNumber)
                .ToListAsync();
        }

        public async Task<ShowSeat?> GetByIdAsync(int id)
        {
            return await _context.ShowSeats
                .Include(ss => ss.Seat)
                .FirstOrDefaultAsync(ss => ss.ShowSeatId == id);
        }

        public async Task<List<ShowSeat>> GetByShowIdAsync(int showId)
        {
            return await _context.ShowSeats
                .AsNoTracking()
                .Include(ss => ss.Seat)
                .Where(ss => ss.ShowId == showId)
                .OrderBy(ss => ss.Seat.RowName)
                .ThenBy(ss => ss.Seat.SeatNumber)
                .ToListAsync();
        }

        public async Task<ShowSeat> AddAsync(ShowSeat showSeat)
        {
            _context.ShowSeats.Add(showSeat);
            await _context.SaveChangesAsync();
            return showSeat;
        }

        public async Task UpdateAsync(ShowSeat showSeat)
        {
            _context.ShowSeats.Update(showSeat);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ShowExistsAsync(int showId)
        {
            return await _context.Shows
                .AnyAsync(s => s.ShowId == showId);
        }

        public async Task<bool> SeatBelongsToScreenAsync(int showId, int seatId)
        {
            return await (
                from show in _context.Shows
                join seat in _context.Seats
                    on show.ScreenId equals seat.ScreenId
                where show.ShowId == showId && seat.SeatId == seatId
                select seat
            ).AnyAsync();
        }

        public async Task<bool> ExistsAsync(
            int showId, int seatId, int? excludeId = null)
        {
            return await _context.ShowSeats.AnyAsync(ss =>
                ss.ShowId == showId &&
                ss.SeatId == seatId &&
                (!excludeId.HasValue || ss.ShowSeatId != excludeId.Value));
        }

        public async Task<bool> HasBookingReferencesAsync(int showSeatId)
        {
            var showSeat = await _context.ShowSeats
                .AsNoTracking()
                .FirstOrDefaultAsync(ss => ss.ShowSeatId == showSeatId);

            if (showSeat == null)
                return false;

            return await _context.BookingSeats.AnyAsync(bs =>
                bs.SeatId == showSeat.SeatId &&
                bs.Booking.ShowId == showSeat.ShowId);
        }

        public async Task<bool> DeleteAsync(ShowSeat showSeat)
        {
            _context.ShowSeats.Remove(showSeat);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}