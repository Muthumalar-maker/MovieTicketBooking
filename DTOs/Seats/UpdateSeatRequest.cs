namespace MovieTicketBooking.DTOs.Seats
{
    public class UpdateSeatRequest
    {
        public int ScreenId { get; set; }
        public string RowName { get; set; } = "";
        public int SeatNumber { get; set; }
        public string SeatType { get; set; } = "";
    }
}