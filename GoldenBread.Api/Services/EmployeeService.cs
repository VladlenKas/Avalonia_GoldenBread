using GoldenBread.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoldenBread.Api.Services
{
    public class EmployeeService(GoldenBreadContext context)
    {
        public async Task<List<Employee>> GetEmployeesAsync()
        {
            return await context.Employees.ToListAsync();
        }
    }
}
