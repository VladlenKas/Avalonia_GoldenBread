using GoldenBread.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoldenBread.Api.Services
{
    public class EmployeeApiService(GoldenBreadContext context)
    {
        // Get List 
        public async Task<List<Employee>> GetAllAsync()
        {
            return await context.Employees.ToListAsync();
        }
    }
}
