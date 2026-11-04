namespace MovieTicketBooking.DTOs.Shows
{
    public class CreateShowRequest
    {
        public int MovieId { get; set; }
        public int ScreenId { get; set; }
        public DateTime StartTime { get; set; }
        public decimal TicketPrice { get; set; }
    }
}