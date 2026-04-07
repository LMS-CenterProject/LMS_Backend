using LMS.Application.Common.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LMS.API.Extensions
{
    public sealed class CurrentUserService(IHttpContextAccessor accessor)
    : ICurrentUserService
    {
        private ClaimsPrincipal? User => accessor.HttpContext?.User;

        public Guid? UserId
        {
            get
            {
                var value = User?.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User?.FindFirstValue(JwtRegisteredClaimNames.Sub);
                return Guid.TryParse(value, out var id) ? id : null;
            }
        }

        public string? Role => User?.FindFirstValue(ClaimTypes.Role);
        public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

        // Add this using at the top of the file:
        // using System.IdentityModel.Tokens.Jwt;
    }
}
