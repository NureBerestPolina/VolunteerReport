using EnRoute.Common.Configuration;
using EnRoute.Infrastructure.Services.Interfaces;
using Google.Apis.Auth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EnRoute.Infrastructure.Services
{
    public class GoogleJwtTokenParser : IGoogleJwtTokenParser
    {

        public Task<GoogleJsonWebSignature.Payload> GetPayloadFromToken(string? token)
        {
            return GoogleJsonWebSignature.ValidateAsync(token);
        }
    }
}
