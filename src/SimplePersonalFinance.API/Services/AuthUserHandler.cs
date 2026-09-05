using SimplePersonalFinance.API.Services.Interfaces;
using SimplePersonalFinance.Core.Interfaces.Services;
using System.Security.Claims;

namespace SimplePersonalFinance.API.Services;

public class AuthUserHandler(IHttpContextAccessor accessor) : IAuthUserHandler, ICurrentUser
{
    public Guid UserId => GetUserId();

    public Guid GetUserId()
    {
        if (!IsAuthenticated())
            return Guid.Empty;

        var userIdClaim = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    private bool IsAuthenticated()
    {
        return accessor.HttpContext?.User.Identity?.IsAuthenticated == true;
    }
}
