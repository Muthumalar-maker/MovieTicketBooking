
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services;
using MovieTicketBooking.Services.Interfaces;
using MovieTicketBooking.Middleware;

namespace MovieTicketBooking
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add Controllers
            builder.Services.AddControllers();

            // Database Connection
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            // Movie
            builder.Services.AddScoped<IMovieRepository, MovieRepository>();
            builder.Services.AddScoped<IMovieService, MovieService>();

            // Theatre
            builder.Services.AddScoped<ITheatreRepository, TheatreRepository>();
            builder.Services.AddScoped<ITheatreService, TheatreService>();

            // Screen
            builder.Services.AddScoped<IScreenRepository, ScreenRepository>();
            builder.Services.AddScoped<IScreenService, ScreenService>();

            // Seat
            builder.Services.AddScoped<ISeatRepository, SeatRepository>();
            builder.Services.AddScoped<ISeatService, SeatService>();

            // Show
            builder.Services.AddScoped<IShowRepository, ShowRepository>();
            builder.Services.AddScoped<IShowService, ShowService>();

            // ShowSeat
            builder.Services.AddScoped<IShowSeatRepository, ShowSeatRepository>();
            builder.Services.AddScoped<IShowSeatService, ShowSeatService>();

            // Booking
            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<IBookingService, BookingService>();

            // Authentication Services
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            // JWT Configuration
            var jwt = builder.Configuration.GetSection("Jwt");
            var jwtKey = jwt["Key"];

            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new InvalidOperationException("JWT Key is missing from configuration.");
            }

            if (string.IsNullOrWhiteSpace(jwt["Issuer"]) ||
                string.IsNullOrWhiteSpace(jwt["Audience"]))
            {
                throw new InvalidOperationException("JWT Issuer or Audience is missing.");
            }

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwt["Issuer"],
                        ValidAudience = jwt["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtKey)),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            builder.Services.AddAuthorization();

            // Exception Handler
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Development Admin Account
            if (app.Environment.IsDevelopment())
            {
                using (var scope = app.Services.CreateScope())
                {
                    var context = scope.ServiceProvider
                        .GetRequiredService<ApplicationDbContext>();

                    var passwordHasher = scope.ServiceProvider
                        .GetRequiredService<IPasswordHasher<User>>();

                    var adminExists = await context.Users
                        .AnyAsync(x => x.Email == "admin@gmail.com");

                    if (!adminExists)
                    {
                        var admin = new User
                        {
                            FullName = "Admin",
                            Email = "admin@gmail.com",
                            Role = "Admin"
                        };

                        admin.PasswordHash = passwordHasher.HashPassword(
                            admin,
                            "Admin@123"
                        );

                        context.Users.Add(admin);
                        await context.SaveChangesAsync();
                    }
                }
            }

            // HTTP Request Pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseExceptionHandler();

            app.UseHttpsRedirection();

            // Authentication must come before Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
