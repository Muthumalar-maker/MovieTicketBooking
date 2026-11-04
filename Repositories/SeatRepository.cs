using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    public class SeatRepository : ISeatRepository
    {
        private readonly ApplicationDbContext _context;

        public SeatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Seat>> GetAllAsync()
        {
            return await _context.Seats
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Seat?> GetByIdAsync(int id)
        {
            return await _context.Seats
                .FirstOrDefaultAsync(s => s.SeatId == id);
        }

        public async Task<Seat> AddAsync(Seat seat)
        {
            _context.Seats.Add(seat);
            await _context.SaveChangesAsync();
            return seat;
        }

        public async Task UpdateAsync(Seat seat)
        {
            _context.Seats.Update(seat);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ScreenExistsAsync(int screenId)
        {
            return await _context.Screens
                .AnyAsync(s => s.ScreenId == screenId);
        }

        public async Task<bool> ExistsByPositionAsync(
            int screenId, string rowName, int seatNumber,
            int? excludeSeatId = null)
        {
            return await _context.Seats.AnyAsync(s =>
                s.ScreenId == screenId &&
                s.RowName == rowName &&
                s.SeatNumber == seatNumber &&
                (!excludeSeatId.HasValue || s.SeatId != excludeSeatId.Value));
        }

        public async Task<bool> HasReferencesAsync(int seatId)
        {
            bool hasShowSeats = await _context.ShowSeats
                .AnyAsync(s => s.SeatId == seatId);

            bool hasBookingSeats = await _context.BookingSeats
                .AnyAsync(b => b.SeatId == seatId);

            return hasShowSeats || hasBookingSeats;
        }

        public async Task<bool> DeleteAsync(Seat seat)
        {
            _context.Seats.Remove(seat);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}