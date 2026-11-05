namespace MovieTicketBooking.Models
{
    public class Booking
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

        public Show Show { get; set; } = null!;
        public List<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();

        public int UserId { get; set; }
        public User User { get; set; } = null!;

    }
}