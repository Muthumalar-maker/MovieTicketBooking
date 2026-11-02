using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.ShowSeats;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowSeatsController : ControllerBase
    {
        private readonly IShowSeatService _showSeatService;

        public ShowSeatsController(IShowSeatService showSeatService)
        {
            _showSeatService = showSeatService;
        }

        // GET: api/ShowSeats
        [HttpGet]
        public async Task<ActionResult<List<ShowSeatResponse>>> GetAll()
        {
            return Ok(await _showSeatService.GetAllAsync());
        }

        // GET: api/ShowSeats/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ShowSeatResponse>> GetById(int id)
        {
            try
            {
                var showSeat = await _showSeatService.GetByIdAsync(id);

                if (showSeat == null)
                    return NotFound(new { message = "Show seat not found." });

                return Ok(showSeat);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/ShowSeats/show/1
        [HttpGet("show/{showId:int}")]
        public async Task<ActionResult<List<ShowSeatResponse>>> GetByShowId(
            int showId)
        {
            try
            {
                var showSeats =
                    await _showSeatService.GetByShowIdAsync(showId);

                return Ok(showSeats);
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

        // POST: api/ShowSeats
        [HttpPost]
        public async Task<ActionResult<ShowSeatResponse>> Create(
            CreateShowSeatRequest request)
        {
            try
            {
                var showSeat =
                    await _showSeatService.CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = showSeat.ShowSeatId },
                    showSeat);
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

        // PUT: api/ShowSeats/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ShowSeatResponse>> Update(
            int id, UpdateShowSeatRequest request)
        {
            try
            {
                var showSeat =
                    await _showSeatService.UpdateAsync(id, request);

                if (showSeat == null)
                    return NotFound(new { message = "Show seat not found." });

                return Ok(showSeat);
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

        // DELETE: api/ShowSeats/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool deleted = await _showSeatService.DeleteAsync(id);

                if (!deleted)
                    return NotFound(new { message = "Show seat not found." });

                return NoContent();
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