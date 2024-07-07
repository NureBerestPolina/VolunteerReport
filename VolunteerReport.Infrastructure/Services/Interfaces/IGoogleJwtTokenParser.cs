using Google.Apis.Auth;
using System.Security.Claims;

namespace EnRoute.Infrastructure.Services.Interfaces
{
    public interface IGoogleJwtTokenParser
    {
        Task<GoogleJsonWebSignature.Payload> GetPayloadFromToken(string? token);
    }
}
