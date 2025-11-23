using GoldenBread.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoldenBread.Api.ApiServices
{
    public class UserApiService(GoldenBreadContext context)
    {
        public async Task<List<User>> GetAllAsync()
        {
            return await context.Users.ToListAsync();
        }
    }
}
