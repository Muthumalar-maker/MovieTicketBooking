using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.Constants;
using MovieTicketBooking.DTOs.Theatres;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TheatresController : ControllerBase
    {
        private readonly ITheatreService _theatreService;

        public TheatresController(ITheatreService theatreService)
        {
            _theatreService = theatreService;
        }

        [Authorize(Roles = UserRoles.Admin)]
        // GET: api/theatres
        [HttpGet]
        public async Task<ActionResult<List<TheatreResponse>>> GetAll()
        {
            var theatres = await _theatreService.GetAllAsync();

            return Ok(theatres);
        }

        [Authorize(Roles = UserRoles.Admin)]
        // GET: api/theatres/1
        [HttpGet("{id:int}")]
        public async Task<ActionResult<TheatreResponse>> GetById(int id)
        {
            try
            {
                var theatre = await _theatreService.GetByIdAsync(id);

                if (theatre == null)
                    return NotFound(new { message = "Theatre not found." });

                return Ok(theatre);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        // POST: api/theatres
        [HttpPost]
        public async Task<ActionResult<TheatreResponse>> Create(
            CreateTheatreRequest request)
        {
            try
            {
                var theatre = await _theatreService.CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = theatre.TheatreId },
                    theatre);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("{id}")]
        // PUT: api/theatres/1
        [HttpPut("{id:int}")]
        public async Task<ActionResult<TheatreResponse>> Update(
            int id,
            UpdateTheatreRequest request)
        {
            try
            {
                var theatre = await _theatreService.UpdateAsync(id, request);

                if (theatre == null)
                    return NotFound(new { message = "Theatre not found." });

                return Ok(theatre);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{id}")]
        // DELETE: api/theatres/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool deleted = await _theatreService.DeleteAsync(id);

                if (!deleted)
                    return NotFound(new { message = "Theatre not found." });

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