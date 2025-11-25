using GoldenBread.Api.Services;
using GoldenBread.Api.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GoldenBread.Domain.Models;

namespace GoldenBread.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ApiControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromServices] UserApiService service)
        {
            try
            {
                var users = await service.GetAllAsync();
                return SuccessWithData(users, MessageHelper.SuccesFromApi);
            }
            catch (Exception ex)
            {
                return ServerError(MessageHelper.ErrorFromApi);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromServices] UserApiService service, int id)
        {
            try
            {
                var result = await service.DeleteAsync(id);
                if (!result)
                {
                    return NotFoundError(MessageHelper.UserNotFound);
                }

                return Success<object>(MessageHelper.UserDeleted);
            }
            catch (Exception ex)
            {
                return ServerError(MessageHelper.ErrorFromApi);
            }
        }
    }
}
