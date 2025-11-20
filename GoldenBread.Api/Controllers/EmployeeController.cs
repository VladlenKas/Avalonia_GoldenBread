using GoldenBread.Api.Helpers;
using GoldenBread.Api.Services;
using GoldenBread.Shared.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoldenBread.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ApiControllerBase
    {
        [HttpGet("getList")]
        public async Task<IActionResult> GetEmployees(
            [FromServices] EmployeeService service)
        {
            try
            {
                var employees = await service.GetEmployeesAsync();
                return Success(employees, MessageHelper.SuccesFromApi);
            }
            catch (Exception ex)
            {
                return ServerError(MessageHelper.ErrorFromApi);
            }
        }
    }
}
