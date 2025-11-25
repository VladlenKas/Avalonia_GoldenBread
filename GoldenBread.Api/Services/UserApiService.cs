using GoldenBread.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GoldenBread.Api.Services
{
    public class UserApiService(GoldenBreadContext context)
    {
        // Get List
        public async Task<List<User>> GetAllAsync()
        {
            return await context.Users.ToListAsync();
        }

        public async Task<User> CreateAsync(User user)
        {
            context.AllUsers.Add(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdateAsync(int id, User user)
        {
            var existingUser = await context.AllUsers.FindAsync(id);
            if (existingUser == null)
                return null;

            existingUser.Firstname = user.Firstname;
            existingUser.Lastname = user.Lastname;
            existingUser.Role = user.Role;
            // Continue!!

            await context.SaveChangesAsync();
            return existingUser;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await context.AllUsers.FindAsync(id);
            if (user == null)
                return false;

            user.Dismissed = 1;
            context.AllUsers.Update(user);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
