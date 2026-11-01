using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // GET: api/Bookings
        // GET: api/Bookings?showId=1
        // GET: api/Bookings?status=Confirmed
        [HttpGet]
        public async Task<ActionResult<List<BookingResponse>>> GetAll(
            [FromQuery] int? showId,
            [FromQuery] string? status)
        {
            try
            {
                var bookings = await _bookingService.GetAllAsync(showId, status);

                return Ok(bookings);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/Bookings/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookingResponse>> GetById(int id)
        {
            try
            {
                var booking = await _bookingService.GetByIdAsync(id);

                if (booking == null)
                    return NotFound(new { message = "Booking not found." });

                return Ok(booking);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/Bookings/Show/1
        [HttpGet("show/{showId:int}")]
        public async Task<ActionResult<List<BookingResponse>>> GetByShowId(
            int showId)
        {
            try
            {
                var bookings = await _bookingService.GetByShowIdAsync(showId);

                return Ok(bookings);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // POST: api/Bookings
        [HttpPost]
        public async Task<ActionResult<BookingResponse>> Create(
            CreateBookingRequest request)
        {
            try
            {
                var booking = await _bookingService.CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = booking.BookingId },
                    booking);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // PUT: api/Bookings/5/Cancel
        [HttpPut("{id:int}/cancel")]
        public async Task<ActionResult<BookingResponse>> Cancel(int id)
        {
            try
            {
                var booking = await _bookingService.CancelAsync(id);

                if (booking == null)
                    return NotFound(new { message = "Booking not found." });

                return Ok(booking);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}