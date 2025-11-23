using GoldenBread.Api.ApiServices;
using GoldenBread.Api.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoldenBread.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ApiControllerBase
    {
        [HttpGet("getList")]
        public async Task<IActionResult> GetAsync([FromServices] UserApiService service)
        {
            try
            {
                var users = await service.GetAllAsync();
                return Success(users, MessageHelper.SuccesFromApi);
            }
            catch (Exception ex)
            {
                return ServerError(MessageHelper.ErrorFromApi);
            }
        }
    }
}
