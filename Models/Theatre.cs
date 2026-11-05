namespace MovieTicketBooking.Models
{
    public class Theatre
    {
        public int TheatreId { get; set; }

        public string Name { get; set; } = "";

        public string Location { get; set; } = "";

        public List<Screen> Screens { get; set; } = new List<Screen>();
    }
}