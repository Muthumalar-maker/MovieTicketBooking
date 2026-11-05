using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.DTOs.Auth;
using MovieTicketBooking.Models;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IJwtService _jwtService;

        public UserService(
            ApplicationDbContext context,
            IPasswordHasher<User> passwordHasher,
            IJwtService jwtService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        // REGISTER
        public async Task RegisterAsync(RegisterDto registerDto)
        {
            var email = registerDto.Email.Trim();

            var existingUser = await _context.Users
                .AnyAsync(x => x.Email == email);

            if (existingUser)
            {
                throw new InvalidOperationException(
                    "Email is already registered.");
            }

            var user = new User
            {
                FullName = registerDto.FullName.Trim(),
                Email = email,
                Role = "Customer"
            };

            user.PasswordHash = _passwordHasher.HashPassword(
                user,
                registerDto.Password
            );

            _context.Users.Add(user);

            await _context.SaveChangesAsync();
        }

        // LOGIN
        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            var email = loginDto.Email.Trim();

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == email);

            if (user == null)
            {
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");
            }

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                loginDto.Password
            );

            if (result == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");
            }

            return _jwtService.GenerateToken(user);
        }
    }
}