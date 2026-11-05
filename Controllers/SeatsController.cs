using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.Constants;
using MovieTicketBooking.DTOs.Seats;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatsController : ControllerBase
    {
        private readonly ISeatService _seatService;

        public SeatsController(ISeatService seatService)
        {
            _seatService = seatService;
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        public async Task<ActionResult<List<SeatResponse>>> GetAll()
        {
            return Ok(await _seatService.GetAllAsync());
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<SeatResponse>> GetById(int id)
        {
            try
            {
                var seat = await _seatService.GetByIdAsync(id);

                if (seat == null)
                    return NotFound(new { message = "Seat not found." });

                return Ok(seat);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [HttpPost]
        public async Task<ActionResult<SeatResponse>> Create(
            CreateSeatRequest request)
        {
            try
            {
                var seat = await _seatService.CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = seat.SeatId },
                    seat);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("{id}")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<SeatResponse>> Update(
            int id, UpdateSeatRequest request)
        {
            try
            {
                var seat = await _seatService.UpdateAsync(id, request);

                if (seat == null)
                    return NotFound(new { message = "Seat not found." });

                return Ok(seat);
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


        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{id}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool deleted = await _seatService.DeleteAsync(id);

                if (!deleted)
                    return NotFound(new { message = "Seat not found." });

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