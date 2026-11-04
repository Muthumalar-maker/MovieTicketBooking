namespace MovieTicketBooking.DTOs.Bookings
{
    public class CreateBookingRequest
    {
        public int ShowId { get; set; }

        public string CustomerName { get; set; } = "";

        public string CustomerEmail { get; set; } = "";

        public string CustomerPhone { get; set; } = "";

        public List<int> SeatIds { get; set; } = new List<int>();
    }
}