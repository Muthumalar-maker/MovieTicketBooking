using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Shows;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowsController : ControllerBase
    {
        private readonly IShowService _showService;

        public ShowsController(IShowService showService)
        {
            _showService = showService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ShowResponse>>> GetAll()
        {
            return Ok(await _showService.GetAllAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ShowResponse>> GetById(int id)
        {
            try
            {
                var show = await _showService.GetByIdAsync(id);

                if (show == null)
                    return NotFound(new { message = "Show not found." });

                return Ok(show);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ShowResponse>> Create(
            CreateShowRequest request)
        {
            try
            {
                var show = await _showService.CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = show.ShowId },
                    show);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ShowResponse>> Update(
            int id, UpdateShowRequest request)
        {
            try
            {
                var show = await _showService.UpdateAsync(id, request);

                if (show == null)
                    return NotFound(new { message = "Show not found." });

                return Ok(show);
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

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool deleted = await _showService.DeleteAsync(id);

                if (!deleted)
                    return NotFound(new { message = "Show not found." });

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