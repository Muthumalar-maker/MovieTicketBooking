using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    public class ScreenRepository : IScreenRepository
    {
        private readonly ApplicationDbContext _context;

        public ScreenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Screen>> GetAllAsync()
        {
            return await _context.Screens
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Screen?> GetByIdAsync(int id)
        {
            return await _context.Screens
                .FirstOrDefaultAsync(s => s.ScreenId == id);
        }

        public async Task<Screen> AddAsync(Screen screen)
        {
            await _context.Screens.AddAsync(screen);
            await _context.SaveChangesAsync();

            return screen;
        }

        public async Task UpdateAsync(Screen screen)
        {
            _context.Screens.Update(screen);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByNameAsync(
            int theatreId,
            string screenName,
            int? excludeScreenId = null)
        {
            return await _context.Screens.AnyAsync(s =>
                s.TheatreId == theatreId &&
                s.ScreenName == screenName &&
                (!excludeScreenId.HasValue ||
                 s.ScreenId != excludeScreenId.Value));
        }

        public async Task<bool> HasSeatsAsync(int screenId)
        {
            return await _context.Seats
                .AnyAsync(s => s.ScreenId == screenId);
        }

        public async Task<bool> HasShowsAsync(int screenId)
        {
            return await _context.Shows
                .AnyAsync(s => s.ScreenId == screenId);
        }

        public async Task<bool> DeleteAsync(Screen screen)
        {
            _context.Screens.Remove(screen);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> TheatreExistsAsync(int theatreId)
        {
            return await _context.Theatres
                .AnyAsync(t => t.TheatreId == theatreId);
        }
    }
}