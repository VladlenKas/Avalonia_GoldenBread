using GoldenBread.Domain.Models;
using GoldenBread.Domain.Requests;
using Microsoft.EntityFrameworkCore;

namespace GoldenBread.Api.Services
{
    public class UserService(GoldenBreadContext context)
    {
        // Get List
        public async Task<List<User>> GetAllAsync()
        {
            return await context.Users.ToListAsync();
        }

        public async Task<User> CreateAsync(User request)
        {
            context.AllUsers.Add(request);
            await context.SaveChangesAsync();
            return request;
        }

        public async Task<User?> UpdateAsync(int id, User request)
        {
            var existingUser = await context.AllUsers.FindAsync(id);
            if (existingUser == null)
                return null;

            existingUser.Firstname = request.Firstname;
            existingUser.Lastname = request.Lastname;
            existingUser.Patronymic = request.Patronymic;
            existingUser.Birthday = request.Birthday;
            existingUser.Email = request.Email;
            existingUser.Password = request.Password;
            existingUser.Role = request.Role;
            existingUser.VerificationStatus = request.VerificationStatus;

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
