using System.Collections.Generic;

namespace MovieTicketBooking.Models
{
    public class Screen
    {
        public int ScreenId { get; set; }

        public int TheatreId { get; set; }

        public string ScreenName { get; set; } = "";

        public Theatre Theatre { get; set; } = null!;

        public List<Seat> Seats { get; set; } = new List<Seat>();

        public List<Show> Shows { get; set; } = new List<Show>();
    }
}