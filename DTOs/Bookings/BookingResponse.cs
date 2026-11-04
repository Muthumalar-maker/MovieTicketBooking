using System;

namespace MovieTicketBooking.DTOs.Bookings
{
    public class BookingResponse
    {
        public int BookingId { get; set; }

        public string BookingReference { get; set; } = "";

        public int ShowId { get; set; }

        public string CustomerName { get; set; } = "";

        public string CustomerEmail { get; set; } = "";

        public string CustomerPhone { get; set; } = "";

        public DateTime BookingDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "";

        public List<BookingSeatResponse> Seats { get; set; }
            = new List<BookingSeatResponse>();
    }
}