

namespace DnTech.API.Controllers
{
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        protected IActionResult HandleResult<T>(OperationResult<T> result)
        {
            if (result.IsSuccess && result.Data != null)
            {
                return Ok(result.Data);
            }

            if (result.IsSuccess && result.Data == null)
            {
                return NotFound();
            }

            return BadRequest(new
            {
                message = result.Error,
                errors = result.Errors
            });
        }

        protected IActionResult HandleResult(OperationResult result)
        {
            if (result.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(new
            {
                message = result.Error,
                errors = result.Errors
            });
        }

        protected Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim ?? Guid.Empty.ToString());
        }

        protected string? GetCurrentUserEmail()
        {
            return User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        }

        protected IEnumerable<string> GetCurrentUserRoles()
        {
            return User.FindAll(System.Security.Claims.ClaimTypes.Role)
                .Select(c => c.Value);
        }
    }
}
