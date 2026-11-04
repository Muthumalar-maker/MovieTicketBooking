
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Data
{
    public static class CustomerSeeder
    {
        public static async Task SeedAsync(
            ApplicationDbContext context,
            IPasswordHasher<User> passwordHasher)
        {
            string email = "customer@gmail.com";

            bool customerExists = await context.Users
                .AnyAsync(x => x.Email == email);

            if (customerExists)
            {
                return;
            }

            var customer = new User
            {
                FullName = "Test Customer",
                Email = email,
                Role = "Customer"
            };

            customer.PasswordHash = passwordHasher.HashPassword(
                customer,
                "Customer@123"
            );

            context.Users.Add(customer);

            await context.SaveChangesAsync();
        }
    }
}
