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

        public async Task<User> CreateAsync(UserRequest request)
        {
            var user = MapToEntity(request);
            context.AllUsers.Add(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdateAsync(int id, UserRequest request)
        {
            var existingUser = await context.AllUsers.FindAsync(id);
            if (existingUser == null)
                return null;

            ApplyRequest(existingUser, request);
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

        private static User MapToEntity(UserRequest request) => new()
        {
            UserId = request.UserId,
            Firstname = request.Firstname,
            Lastname = request.Lastname,
            Patronymic = request.Patronymic,
            Birthday = request.Birthday,
            Email = request.Email,
            Password = request.Password,
            Role = request.Role,
            AccountType = request.AccountType,
            VerificationStatus = request.VerificationStatus,
            Dismissed = request.Dismissed
        };

        private static void ApplyRequest(User existing, UserRequest request)
        {
            existing.Firstname = request.Firstname;
            existing.Lastname = request.Lastname;
            existing.Patronymic = request.Patronymic;
            existing.Birthday = request.Birthday;
            existing.Email = request.Email;
            existing.Password = request.Password;
            existing.Role = request.Role;
            existing.AccountType = request.AccountType;
            existing.VerificationStatus = request.VerificationStatus;
            existing.Dismissed = request.Dismissed;
        }
    }
}
