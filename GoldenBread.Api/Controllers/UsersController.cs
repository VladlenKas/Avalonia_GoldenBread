using GoldenBread.Api.Helpers;
using GoldenBread.Api.Services;
using GoldenBread.Domain.Models;
using GoldenBread.Domain.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoldenBread.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ApiControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromServices] UserService service)
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
        public async Task<IActionResult> DeleteAsync([FromServices] UserService service, int id)
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

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromServices] UserService service, [FromBody] UserRequest user)
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
        public async Task<IActionResult> UpdateAsync([FromServices] UserService service, int id, [FromBody] UserRequest user)
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
    }
}
