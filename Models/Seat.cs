using System.Collections.Generic;

namespace MovieTicketBooking.Models
{
    public class Seat
    {
        public int SeatId { get; set; }

        public int ScreenId { get; set; }

        public string RowName { get; set; } = "";

        public int SeatNumber { get; set; }

        public string SeatType { get; set; } = "";

        public Screen Screen { get; set; } = null!;

        public List<ShowSeat> ShowSeats { get; set; } = new List<ShowSeat>();

        public List<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
    }
}