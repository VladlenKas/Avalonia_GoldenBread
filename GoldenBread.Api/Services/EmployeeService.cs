using GoldenBread.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GoldenBread.Api.Services
{
    public class EmployeeService(GoldenBreadContext context)
    {
        // Get List 
        public async Task<List<Employee>> GetAllAsync()
        {
            return await context.Employees.ToListAsync();
        }
    }
}
