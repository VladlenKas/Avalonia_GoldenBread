using GoldenBread.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoldenBread.Api.Services
{
    public class AuthorizationApiService(GoldenBreadContext context)
    {
        // Login
        public async Task<User> LoginAsync(string email, string password)
        {
            var user = await context.Users.SingleOrDefaultAsync(u =>
                    u.Email == email &&
                    u.Password == password);

            return user;
        }
    }
}
