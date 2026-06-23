using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.Contracts
{
    public interface IUserContext
    {
        Guid UserId { get; }
    }

    public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
    {
        public Guid UserId =>
            Guid.TryParse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id)
                ? id
                : Guid.Empty;
    }
}
