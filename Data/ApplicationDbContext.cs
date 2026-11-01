
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Theatre> Theatres { get; set; }
        public DbSet<Screen> Screens { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Show> Shows { get; set; }
        public DbSet<ShowSeat> ShowSeats { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingSeat> BookingSeats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Movie Configuration
            modelBuilder.Entity<Movie>(entity =>
            {
                entity.HasKey(m => m.MovieId);

                entity.Property(m => m.Title)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(m => m.Description)
                      .HasMaxLength(500);

                entity.Property(m => m.DurationMinutes)
                      .IsRequired();

                entity.Property(m => m.ReleaseDate)
                      .HasColumnType("date");

                entity.Property(m => m.Language)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(m => m.Genre)
                      .IsRequired()
                      .HasMaxLength(50);
            });
        }
    }
}
