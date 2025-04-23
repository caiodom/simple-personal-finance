using SimplePersonalFinance.API.Services.Interfaces;
using System.Security.Claims;

namespace SimplePersonalFinance.API.Services
{
    public class AuthUserHandler(IHttpContextAccessor accessor) : IAuthUserHandler
    {
        public Guid GetUserId()
        {
            return IsAuthenticated() ?
                Guid.Parse(accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty) :
                Guid.Empty;
        }

        private bool IsAuthenticated()
        {
            return accessor.HttpContext?.User.Identity?.IsAuthenticated == true;
        }
    }
}
