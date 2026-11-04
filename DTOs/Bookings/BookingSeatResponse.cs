namespace MovieTicketBooking.DTOs.Bookings
{
    public class BookingSeatResponse
    {
        public int SeatId { get; set; }

        public string SeatLabel { get; set; } = "";

        public string SeatType { get; set; } = "";

        public decimal Price { get; set; }
    }
}