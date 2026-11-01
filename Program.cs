using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Repositories;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services;
using MovieTicketBooking.Services.Interfaces;
using MovieTicketBooking.Middleware;


namespace MovieTicketBooking
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add Controllers
            builder.Services.AddControllers();

            // Configure Database Connection
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register Movie Repository
            builder.Services.AddScoped<IMovieRepository, MovieRepository>();

            // Register Movie Service
            //builder.Services.AddScoped<IMovieService, MovieService>();

            builder.Services.AddScoped<MovieTicketBooking.Services.Interfaces.IMovieService, MovieTicketBooking.Services.MovieService>();

            // Add API Explorer
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddScoped<ITheatreRepository, TheatreRepository>();
            builder.Services.AddScoped<ITheatreService, TheatreService>();


            builder.Services.AddScoped<IScreenRepository, ScreenRepository>();
            builder.Services.AddScoped<IScreenService, ScreenService>();

            builder.Services.AddScoped<ISeatRepository, SeatRepository>();
            builder.Services.AddScoped<ISeatService, SeatService>();

            builder.Services.AddScoped<IShowRepository, ShowRepository>();
            builder.Services.AddScoped<IShowService, ShowService>();

            builder.Services.AddScoped<IShowSeatRepository, ShowSeatRepository>();
            builder.Services.AddScoped<IShowSeatService, ShowSeatService>();

            builder.Services.AddScoped<IShowSeatRepository, ShowSeatRepository>();
            builder.Services.AddScoped<IShowSeatService, ShowSeatService>();

            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<IBookingService, BookingService>();

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();


            // Add Swagger
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure HTTP Request Pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseExceptionHandler();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}