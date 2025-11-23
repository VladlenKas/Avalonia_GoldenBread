using GoldenBread.Api.Helpers;
using GoldenBread.Api.Services;
using GoldenBread.Shared.Entities;
using GoldenBread.Shared.Requests;
using Microsoft.AspNetCore.Mvc;

namespace GoldenBread.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ApiControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Login([FromServices] AuthorizationApiService service, [FromBody] LoginUser request)
        {
            try
            {
                var user = await service.LoginAsync(request.Login, request.Password);
                if (user == null)
                {
                    return NotFoundError(MessageHelper.IncorrectData);
                }

                return user.VerificationStatus switch
                {
                    VerificationStatus.Pending => ForbidError(MessageHelper.PendingStatus),
                    VerificationStatus.Approved => SuccessWithData(user, MessageHelper.CorrectData(user)),
                    VerificationStatus.Rejected => ForbidError(MessageHelper.RejectedStatus),
                    VerificationStatus.Suspended => ForbidError(MessageHelper.SuspendedStatus),
                    _ => BadRequestError(MessageHelper.UnknownStatus),
                };
            }
            catch (Exception ex) 
            {
                return ServerError(MessageHelper.ErrorFromApi);
            }
        }
    }
}
