using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    public class ShowRepository : IShowRepository
    {
        private readonly ApplicationDbContext _context;

        public ShowRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Show>> GetAllAsync()
        {
            return await _context.Shows
                .AsNoTracking()
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<Show?> GetByIdAsync(int id)
        {
            return await _context.Shows
                .FirstOrDefaultAsync(s => s.ShowId == id);
        }

        public async Task<int?> GetMovieDurationAsync(int movieId)
        {
            return await _context.Movies
                .Where(m => m.MovieId == movieId)
                .Select(m => (int?)m.DurationMinutes)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ScreenExistsAsync(int screenId)
        {
            return await _context.Screens
                .AnyAsync(s => s.ScreenId == screenId);
        }

        public async Task<List<int>> GetScreenSeatIdsAsync(int screenId)
        {
            return await _context.Seats
                .Where(s => s.ScreenId == screenId)
                .Select(s => s.SeatId)
                .ToListAsync();
        }

        public async Task<bool> HasOverlapAsync(
            int screenId, DateTime startTime, DateTime endTime,
            int? excludeShowId = null)
        {
            return await _context.Shows.AnyAsync(s =>
                s.ScreenId == screenId &&
                s.StartTime < endTime &&
                s.EndTime > startTime &&
                (!excludeShowId.HasValue || s.ShowId != excludeShowId.Value));
        }

        public async Task<Show> AddWithSeatsAsync(Show show, List<int> seatIds)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Shows.Add(show);
                await _context.SaveChangesAsync();

                var showSeats = seatIds.Select(seatId => new ShowSeat
                {
                    ShowId = show.ShowId,
                    SeatId = seatId,
                    Status = "Available"
                }).ToList();

                _context.ShowSeats.AddRange(showSeats);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return show;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateAsync(Show show)
        {
            _context.Shows.Update(show);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasBookingsAsync(int showId)
        {
            return await _context.Bookings
                .AnyAsync(b => b.ShowId == showId);
        }

        public async Task<bool> DeleteAsync(Show show)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var showSeats = await _context.ShowSeats
                    .Where(ss => ss.ShowId == show.ShowId)
                    .ToListAsync();

                _context.ShowSeats.RemoveRange(showSeats);
                _context.Shows.Remove(show);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task UpdateWithSeatsAsync(Show show, List<int> seatIds)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var existingShowSeats = await _context.ShowSeats
                    .Where(ss => ss.ShowId == show.ShowId)
                    .ToListAsync();

                _context.ShowSeats.RemoveRange(existingShowSeats);

                _context.Shows.Update(show);
                await _context.SaveChangesAsync();

                var newShowSeats = seatIds.Select(seatId => new ShowSeat
                {
                    ShowId = show.ShowId,
                    SeatId = seatId,
                    Status = "Available"
                }).ToList();

                _context.ShowSeats.AddRange(newShowSeats);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}