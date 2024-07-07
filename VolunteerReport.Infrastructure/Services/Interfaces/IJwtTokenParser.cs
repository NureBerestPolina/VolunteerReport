using System.Security.Claims;

namespace EnRoute.Infrastructure.Services.Interfaces
{
    public interface IJwtTokenParser
    {
        ClaimsPrincipal? GetPrincipalFromToken(string? token);
    }
}
