using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    public class TheatreRepository : ITheatreRepository
    {
        private readonly ApplicationDbContext _context;

        public TheatreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Theatre>> GetAllAsync()
        {
            return await _context.Theatres
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Theatre?> GetByIdAsync(int id)
        {
            return await _context.Theatres
                .FirstOrDefaultAsync(t => t.TheatreId == id);
        }

        public async Task<Theatre> AddAsync(Theatre theatre)
        {
            await _context.Theatres.AddAsync(theatre);
            await _context.SaveChangesAsync();

            return theatre;
        }

        public async Task<bool> HasScreensAsync(int theatreId)
        {
            return await _context.Screens
                .AnyAsync(s => s.TheatreId == theatreId);
        }

        public async Task<bool> DeleteAsync(Theatre theatre)
        {
            _context.Theatres.Remove(theatre);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task UpdateAsync(Theatre theatre)
        {
            _context.Theatres.Update(theatre);

            await _context.SaveChangesAsync();
        }
    }
}