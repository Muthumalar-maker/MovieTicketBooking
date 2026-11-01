
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

                // Theatre - Screen
                modelBuilder.Entity<Screen>()
                    .HasOne(s => s.Theatre)
                    .WithMany(t => t.Screens)
                    .HasForeignKey(s => s.TheatreId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Screen - Seat
                modelBuilder.Entity<Seat>()
                    .HasOne(s => s.Screen)
                    .WithMany(sc => sc.Seats)
                    .HasForeignKey(s => s.ScreenId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Movie - Show
                modelBuilder.Entity<Show>()
                    .HasOne(s => s.Movie)
                    .WithMany(m => m.Shows)
                    .HasForeignKey(s => s.MovieId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Screen - Show
                modelBuilder.Entity<Show>()
                    .HasOne(s => s.Screen)
                    .WithMany(sc => sc.Shows)
                    .HasForeignKey(s => s.ScreenId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Show - ShowSeat
                modelBuilder.Entity<ShowSeat>()
                    .HasOne(ss => ss.Show)
                    .WithMany(s => s.ShowSeats)
                    .HasForeignKey(ss => ss.ShowId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Seat - ShowSeat
                modelBuilder.Entity<ShowSeat>()
                    .HasOne(ss => ss.Seat)
                    .WithMany(s => s.ShowSeats)
                    .HasForeignKey(ss => ss.SeatId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Show - Booking
                modelBuilder.Entity<Booking>()
                    .HasOne(b => b.Show)
                    .WithMany(s => s.Bookings)
                    .HasForeignKey(b => b.ShowId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Booking - BookingSeat
                modelBuilder.Entity<BookingSeat>()
                    .HasOne(bs => bs.Booking)
                    .WithMany(b => b.BookingSeats)
                    .HasForeignKey(bs => bs.BookingId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Seat - BookingSeat
                modelBuilder.Entity<BookingSeat>()
                    .HasOne(bs => bs.Seat)
                    .WithMany(s => s.BookingSeats)
                    .HasForeignKey(bs => bs.SeatId)
                    .OnDelete(DeleteBehavior.NoAction);

                //Screen
                modelBuilder.Entity<Screen>()
                .HasIndex(s => new { s.TheatreId, s.ScreenName })
                .IsUnique();

                modelBuilder.Entity<Seat>()
                    .HasIndex(s => new { s.ScreenId, s.RowName, s.SeatNumber })
                    .IsUnique();
            });

            // Show Configuration
            modelBuilder.Entity<Show>(entity =>
            {
                entity.Property(s => s.TicketPrice)
                      .HasPrecision(10, 2);
            });

            // Booking Configuration
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.Property(b => b.TotalAmount)
                      .HasPrecision(10, 2);
            });

            // BookingSeat Configuration
            modelBuilder.Entity<BookingSeat>(entity =>
            {
                entity.Property(bs => bs.Price)
                      .HasPrecision(10, 2);
            });
        }
    }
}
