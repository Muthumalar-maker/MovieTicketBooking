using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.Constants;
using MovieTicketBooking.DTOs.Screens;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScreensController : ControllerBase
    {
        private readonly IScreenService _screenService;

        public ScreensController(IScreenService screenService)
        {
            _screenService = screenService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ScreenResponse>>> GetAll()
        {
            return Ok(await _screenService.GetAllAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ScreenResponse>> GetById(int id)
        {
            try
            {
                var screen = await _screenService.GetByIdAsync(id);

                if (screen == null)
                    return NotFound(new { message = "Screen not found." });

                return Ok(screen);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [HttpPost]
        public async Task<ActionResult<ScreenResponse>> Create(
            CreateScreenRequest request)
        {
            try
            {
                var screen = await _screenService.CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = screen.ScreenId },
                    screen);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("{id}")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ScreenResponse>> Update(
            int id, UpdateScreenRequest request)
        {
            try
            {
                var screen = await _screenService.UpdateAsync(id, request);

                if (screen == null)
                    return NotFound(new { message = "Screen not found." });

                return Ok(screen);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{id}")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool deleted = await _screenService.DeleteAsync(id);

                if (!deleted)
                    return NotFound(new { message = "Screen not found." });

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