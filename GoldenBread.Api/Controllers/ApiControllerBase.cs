using GoldenBread.Domain.Responses;
using Microsoft.AspNetCore.Mvc;

namespace GoldenBread.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiControllerBase : ControllerBase
    {
        // == Method for Success Operation ==
        protected IActionResult Success<T>(string message)
        {
            return Ok(new ApiResponse<T>
            {
                IsSuccess = true,
                Data = default,
                Message = message
            });
        }

        protected IActionResult SuccessWithData<T>(T data, string message)
        {
            return Ok(new ApiResponse<T>
            {
                IsSuccess = true,
                Data = data,
                Message = message
            });
        }

        // == Method for Error Operation with status-code ==
        protected IActionResult Error<T>(string message, int statusCode = 400)
        {
            var response = new ApiResponse<T>
            {
                IsSuccess = false,
                Data = default,
                Message = message
            };

            return statusCode switch
            {
                400 => BadRequest(response),
                404 => NotFound(response),
                500 => StatusCode(500, response),
                _ => StatusCode(statusCode, response)
            };
        }  

        // == Short Methods for quick access
        protected IActionResult BadRequestError(string message) => Error<object>(message, 400);

        protected IActionResult ForbidError(string message) => Error<object>(message, 403);

        protected IActionResult NotFoundError(string message) => Error<object>(message, 404);

        protected IActionResult ServerError(string message) => Error<object>(message, 500);
    }
}
