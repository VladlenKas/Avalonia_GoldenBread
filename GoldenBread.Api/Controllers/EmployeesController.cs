using GoldenBread.Api.Helpers;
using GoldenBread.Api.Services;
using GoldenBread.Domain.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoldenBread.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ApiControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetEmployees([FromServices] EmployeeApiService service)
        {
            try
            {
                var employees = await service.GetAllAsync();
                return SuccessWithData(employees, MessageHelper.SuccesFromApi);
            }
            catch (Exception ex)
            {
                return ServerError(MessageHelper.ErrorFromApi);
            }
        }
    }
}
