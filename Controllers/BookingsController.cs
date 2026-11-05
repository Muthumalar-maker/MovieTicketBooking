
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.Constants;
using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Services.Interfaces;
using System.Security.Claims;

namespace MovieTicketBooking.Controllers
{
    [Authorize]
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
        // Admin can view all bookings
        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        public async Task<ActionResult<List<BookingResponse>>> GetAll(
            [FromQuery] int? showId,
            [FromQuery] string? status)
        {
            var bookings = await _bookingService.GetAllAsync(
                showId, status, null);

            return Ok(bookings);
        }

        // GET: api/Bookings/5
        // Admin can view booking details
        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookingResponse>> GetById(int id)
        {
            var booking = await _bookingService.GetByIdAsync(id, null);

            if (booking == null)
                return NotFound(new { message = "Booking not found." });

            return Ok(booking);
        }

        // GET: api/Bookings/show/1
        // Customer can view their own bookings for a show
        [HttpGet("show/{showId:int}")]
        public async Task<ActionResult<List<BookingResponse>>> GetByShowId(
            int showId)
        {
            if (!TryGetCurrentUserId(out int userId))
                return Unauthorized();

            int? filterUserId = User.IsInRole(UserRoles.Admin)
                ? null
                : userId;

            try
            {
                var bookings = await _bookingService.GetByShowIdAsync(
                    showId, filterUserId);

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
        // Create booking for logged-in customer
        [HttpPost]
        public async Task<ActionResult<BookingResponse>> Create(
            [FromBody] CreateBookingRequest request)
        {
            if (!TryGetCurrentUserId(out int userId))
                return Unauthorized();

            var customerName = User.FindFirstValue(ClaimTypes.Name);
            var customerEmail = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrWhiteSpace(customerName) ||
                string.IsNullOrWhiteSpace(customerEmail))
            {
                return Unauthorized(new
                {
                    message = "Required user information is missing from the token."
                });
            }

            try
            {
                var booking = await _bookingService.CreateAsync(
                    request,
                    userId,
                    customerName,
                    customerEmail);

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

        // PUT: api/Bookings/5/cancel
        // Customer can cancel only their own booking
        [HttpPut("{id:int}/cancel")]
        public async Task<ActionResult<BookingResponse>> Cancel(int id)
        {
            if (!TryGetCurrentUserId(out int userId))
                return Unauthorized();

            int? filterUserId = User.IsInRole(UserRoles.Admin)
                ? null
                : userId;

            try
            {
                var booking = await _bookingService.CancelAsync(
                    id, filterUserId);

                if (booking == null)
                    return NotFound(new
                    {
                        message = "Booking not found or you are not authorized to cancel it."
                    });

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

        // Read logged-in user ID from JWT token
        private bool TryGetCurrentUserId(out int userId)
        {
            var userIdClaim = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            return int.TryParse(userIdClaim, out userId)
                && userId > 0;
        }
    }
}
