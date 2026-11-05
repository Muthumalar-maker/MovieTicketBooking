
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
using MovieTicketBooking.Constants;

using Scalar.AspNetCore;

namespace MovieTicketBooking
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ==========================================
            // 1. ADD CONTROLLERS
            // ==========================================

            builder.Services.AddControllers();

            // ==========================================
            // 2. DATABASE CONNECTION
            // ==========================================

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            // ==========================================
            // 3. MOVIE SERVICES
            // ==========================================

            builder.Services.AddScoped<IMovieRepository, MovieRepository>();
            builder.Services.AddScoped<IMovieService, MovieService>();

            // ==========================================
            // 4. THEATRE SERVICES
            // ==========================================

            builder.Services.AddScoped<ITheatreRepository, TheatreRepository>();
            builder.Services.AddScoped<ITheatreService, TheatreService>();

            // ==========================================
            // 5. SCREEN SERVICES
            // ==========================================

            builder.Services.AddScoped<IScreenRepository, ScreenRepository>();
            builder.Services.AddScoped<IScreenService, ScreenService>();

            // ==========================================
            // 6. SEAT SERVICES
            // ==========================================

            builder.Services.AddScoped<ISeatRepository, SeatRepository>();
            builder.Services.AddScoped<ISeatService, SeatService>();

            // ==========================================
            // 7. SHOW SERVICES
            // ==========================================

            builder.Services.AddScoped<IShowRepository, ShowRepository>();
            builder.Services.AddScoped<IShowService, ShowService>();

            // ==========================================
            // 8. SHOW SEAT SERVICES
            // ==========================================

            builder.Services.AddScoped<IShowSeatRepository, ShowSeatRepository>();
            builder.Services.AddScoped<IShowSeatService, ShowSeatService>();

            // ==========================================
            // 9. BOOKING SERVICES
            // ==========================================

            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<IBookingService, BookingService>();

            // ==========================================
            // 10. AUTHENTICATION SERVICES
            // ==========================================

            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            // ==========================================
            // 11. JWT CONFIGURATION
            // ==========================================

            var jwt = builder.Configuration.GetSection("Jwt");
            var jwtKey = jwt["Key"];

            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new InvalidOperationException(
                    "JWT Key is missing from configuration.");
            }

            if (string.IsNullOrWhiteSpace(jwt["Issuer"]) ||
                string.IsNullOrWhiteSpace(jwt["Audience"]))
            {
                throw new InvalidOperationException(
                    "JWT Issuer or Audience is missing.");
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

            // ==========================================
            // 12. CORS CONFIGURATION
            // ==========================================

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AngularPolicy", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // ==========================================
            // 13. EXCEPTION HANDLER
            // ==========================================

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            // ==========================================
            // 14. OPENAPI AND SCALAR
            // ==========================================

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddOpenApi();

            // ==========================================
            // BUILD APPLICATION
            // ==========================================

            var app = builder.Build();

            // ==========================================
            // 15. DEVELOPMENT ADMIN AND CUSTOMER SEEDER
            // ==========================================

            if (app.Environment.IsDevelopment())
            {
                using (var scope = app.Services.CreateScope())
                {
                    var context = scope.ServiceProvider
                        .GetRequiredService<ApplicationDbContext>();

                    var passwordHasher = scope.ServiceProvider
                        .GetRequiredService<IPasswordHasher<User>>();

                    var admin = await context.Users
                        .FirstOrDefaultAsync(x => x.Email == "admin@gmail.com");

                    if (admin != null)
                    {
                        admin.PasswordHash = passwordHasher.HashPassword(
                            admin,
                            "Admin@123");

                        await context.SaveChangesAsync();

                        Console.WriteLine(
                            "Admin password reset successfully!");
                    }

                    // Customer Account Seeder
                    await CustomerSeeder.SeedAsync(context, passwordHasher);
                }
            }

            // ==========================================
            // 16. DEVELOPMENT API DOCUMENTATION
            // ==========================================

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            // ==========================================
            // 17. MIDDLEWARE PIPELINE
            // ==========================================

            // Exception Handler
            app.UseExceptionHandler();

            // HTTPS Redirection
            app.UseHttpsRedirection();

            // CORS
            app.UseCors("AngularPolicy");

            // Routing
            app.UseRouting();

            // Endpoint Metadata Diagnostics
            if (app.Environment.IsDevelopment())
            {
                app.UseMiddleware<EndpointMetadataMiddleware>();
            }

            // Authentication
            app.UseAuthentication();

            // Authentication Diagnostics
            if (app.Environment.IsDevelopment())
            {
                app.UseMiddleware<AuthenticationDiagnosticsMiddleware>();

                app.UseMiddleware<AuthorizationDiagnosticsMiddleware>();
            }

            // Authorization
            app.UseAuthorization();

            // ==========================================
            // 18. MAP CONTROLLERS
            // ==========================================

            app.MapControllers();

            // ==========================================
            // 19. RUN APPLICATION
            // ==========================================

            app.Run();
        }
    }
}
