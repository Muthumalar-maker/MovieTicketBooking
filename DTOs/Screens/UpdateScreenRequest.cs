namespace MovieTicketBooking.DTOs.Screens
{
    public class UpdateScreenRequest
    {
        public int TheatreId { get; set; }

        public string ScreenName { get; set; } = "";
    }
}