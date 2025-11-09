using GoldenBread.Api.Services;
using GoldenBread.Shared.Entities;
using GoldenBread.Shared.Enums.User;
using GoldenBread.Shared.Responses;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoldenBread.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpPost("login")]
        public async Task<ActionResult> Login(
            [FromServices] UserService userService, 
            [FromBody] LoginUser request)
        {
            try
            {
                var user = await userService.LoginAsync(request.Login, request.Password);

                if (user == null)
                {
                    return NotFound(new ApiResponse<User>
                    {
                        IsSuccess = false,
                        Data = null,
                        Message = "Введены неверные учётные данные"
                    });
                }

                return Ok(new ApiResponse<User>
                {
                    IsSuccess = true,
                    Data = user,
                    Message = $"Вход выполнен успешно. Добро пожаловать в систему" +
                    $"\n\nПользователь: {user.Lastname}\n" +
                    $"Должность: {user.Role.Value.Humanize()}"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<User>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Произошла ошибка на стороне сервера"
                });
            }
        }
    }
}
