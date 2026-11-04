namespace MovieTicketBooking.DTOs.Screens
{
    public class CreateScreenRequest
    {
        public int TheatreId { get; set; }

        public string ScreenName { get; set; } = "";
    }
}