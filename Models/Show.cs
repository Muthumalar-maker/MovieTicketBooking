
using System;

namespace MovieTicketBooking.Models
{
    public class Show
    {
        public int ShowId { get; set; }

        // Foreign Keys
        public int MovieId { get; set; }
        public int ScreenId { get; set; }

        // Show Details
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public decimal TicketPrice { get; set; }

        // Navigation Properties
        public Movie Movie { get; set; } = null!;
        public Screen Screen { get; set; } = null!;

        public List<ShowSeat> ShowSeats { get; set; } = new List<ShowSeat>();
        public List<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
