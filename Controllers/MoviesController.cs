using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Movies;
using MovieTicketBooking.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using MovieTicketBooking.Constants;

namespace MovieTicketBooking.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        // GET: api/movies
        [HttpGet]
        public async Task<ActionResult<PagedMovieResponse>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? title = null)
        {
            try
            {
                var movies = await _movieService.GetAllAsync(
                    pageNumber,
                    pageSize,
                    title);

                return Ok(movies);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/movies/1
        [HttpGet("{id:int}")]
        public async Task<ActionResult<MovieResponse>> GetById(int id)
        {
            try
            {
                var movie = await _movieService.GetByIdAsync(id);

                if (movie == null)
                    return NotFound(new { message = "Movie not found." });

                return Ok(movie);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/movies/search?title=leo
        [HttpGet("search")]
        public async Task<ActionResult<List<MovieResponse>>> Search(
            [FromQuery] string title)
        {
            try
            {
                var movies = await _movieService.SearchByTitleAsync(title);

                return Ok(movies);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        // POST: api/movies
        [HttpPost]
        public async Task<ActionResult<MovieResponse>> Create(
            CreateMovieRequest request)
        {
            try
            {
                var movie = await _movieService.CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = movie.MovieId },
                    movie);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("{id}")]
        // PUT: api/movies/1
        [HttpPut("{id:int}")]
        public async Task<ActionResult<MovieResponse>> Update(
            int id,
            UpdateMovieRequest request)
        {
            try
            {
                var movie = await _movieService.UpdateAsync(id, request);

                if (movie == null)
                    return NotFound(new { message = "Movie not found." });

                return Ok(movie);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{id}")]
        // DELETE: api/movies/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool deleted = await _movieService.DeleteAsync(id);

                if (!deleted)
                    return NotFound(new { message = "Movie not found." });

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