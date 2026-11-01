using MovieTicketBooking.DTOs.Movies;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    // public class MovieService : IMovieService
    public class MovieService : MovieTicketBooking.Services.Interfaces.IMovieService
    {
        private readonly IMovieRepository _movieRepository;

        public MovieService(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        public async Task<PagedMovieResponse> GetAllAsync(
            int pageNumber,
            int pageSize,
            string? title)
        {
            if (pageNumber < 1)
                throw new ArgumentException("Page number must be greater than zero.");

            if (pageSize < 1 || pageSize > 100)
                throw new ArgumentException("Page size must be between 1 and 100.");

            var result = await _movieRepository.GetPagedAsync(
                pageNumber,
                pageSize,
                title);

            return new PagedMovieResponse
            {
                Items = result.Movies.Select(MapToResponse).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<MovieResponse?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid movie ID.");

            var movie = await _movieRepository.GetByIdAsync(id);

            if (movie == null)
                return null;

            return MapToResponse(movie);
        }

        public async Task<List<MovieResponse>> SearchByTitleAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Movie title is required.");

            var movies = await _movieRepository.SearchByTitleAsync(title.Trim());

            return movies.Select(MapToResponse).ToList();
        }

        public async Task<MovieResponse> CreateAsync(CreateMovieRequest request)
        {
            ValidateMovie(
                request.Title,
                request.Description,
                request.DurationMinutes,
                request.Language,
                request.Genre);

            var movie = new Movie
            {
                Title = request.Title.Trim(),
                Description = request.Description?.Trim(),
                DurationMinutes = request.DurationMinutes,
                ReleaseDate = request.ReleaseDate,
                Language = request.Language.Trim(),
                Genre = request.Genre.Trim()
            };

            var createdMovie = await _movieRepository.AddAsync(movie);

            return MapToResponse(createdMovie);
        }

        public async Task<MovieResponse?> UpdateAsync(
            int id,
            UpdateMovieRequest request)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid movie ID.");

            ValidateMovie(
                request.Title,
                request.Description,
                request.DurationMinutes,
                request.Language,
                request.Genre);

            var movie = await _movieRepository.GetByIdAsync(id);

            if (movie == null)
                return null;

            movie.Title = request.Title.Trim();
            movie.Description = request.Description?.Trim();
            movie.DurationMinutes = request.DurationMinutes;
            movie.ReleaseDate = request.ReleaseDate;
            movie.Language = request.Language.Trim();
            movie.Genre = request.Genre.Trim();

            await _movieRepository.UpdateAsync(movie);

            return MapToResponse(movie);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid movie ID.");

            var movie = await _movieRepository.GetByIdAsync(id);

            if (movie == null)
                return false;

            bool hasShows = await _movieRepository.HasShowsAsync(id);

            if (hasShows)
            {
                throw new InvalidOperationException(
                    "This movie cannot be deleted because shows are associated with it.");
            }

            await _movieRepository.DeleteAsync(movie);

            return true;
        }

        private static void ValidateMovie(
            string title,
            string? description,
            int durationMinutes,
            string language,
            string genre)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Movie title is required.");

            if (title.Trim().Length > 150)
                throw new ArgumentException("Movie title cannot exceed 150 characters.");

            if (description?.Length > 500)
                throw new ArgumentException("Description cannot exceed 500 characters.");

            if (durationMinutes <= 0)
                throw new ArgumentException("Duration must be greater than zero.");

            if (string.IsNullOrWhiteSpace(language))
                throw new ArgumentException("Language is required.");

            if (language.Trim().Length > 50)
                throw new ArgumentException("Language cannot exceed 50 characters.");

            if (string.IsNullOrWhiteSpace(genre))
                throw new ArgumentException("Genre is required.");

            if (genre.Trim().Length > 50)
                throw new ArgumentException("Genre cannot exceed 50 characters.");
        }

        private static MovieResponse MapToResponse(Movie movie)
        {
            return new MovieResponse
            {
                MovieId = movie.MovieId,
                Title = movie.Title,
                Description = movie.Description,
                DurationMinutes = movie.DurationMinutes,
                ReleaseDate = movie.ReleaseDate,
                Language = movie.Language,
                Genre = movie.Genre
            };
        }
    }
}