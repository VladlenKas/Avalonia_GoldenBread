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

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromServices] UserApiService service, [FromBody] User user)
        {
            try
            {
                var createdUser = await service.CreateAsync(user);
                return SuccessWithData(createdUser, MessageHelper.UserCreated);
            }
            catch (Exception ex)
            {
                return ServerError(MessageHelper.ErrorFromApi);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromServices] UserApiService service, int id, [FromBody] User user)
        {
            try
            {
                var result = await service.UpdateAsync(id, user);
                if (result == null)
                {
                    return NotFoundError(MessageHelper.UserNotFound);
                }

                return SuccessWithData(result, MessageHelper.UserUpdated);
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
