using GoldenBread.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoldenBread.Api.ApiServices
{
    public class EmployeeApiService(GoldenBreadContext context)
    {
        public async Task<List<Employee>> GetEmployeesAsync()
        {
            return await context.Employees.ToListAsync();
        }
    }
}
