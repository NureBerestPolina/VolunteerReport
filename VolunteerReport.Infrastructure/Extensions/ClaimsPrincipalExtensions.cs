using EnRoute.Infrastructure.Constants;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EnRoute.Infrastructure.Extentions
{
    public static class ClaimsPrincipalExtensions
    {
        public static (string Name, string Email) GetGoogleJwtClaims(this ClaimsPrincipal claims)
        {
            var name = claims.FindFirst(c => c.Type.Equals(JwtClaims.Name, StringComparison.InvariantCultureIgnoreCase))!.Value;
            var email = claims.FindFirst(c => c.Type.Equals(JwtClaims.Email, StringComparison.InvariantCultureIgnoreCase))!.Value;
            
            return (name, email);
        }
    }
}
