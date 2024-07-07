using EnRoute.Infrastructure.Commands;
using VolunteerReport.Domain.Models;

namespace VolunteerReport.Infrastructure.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(string Token, string RefreshToken)> GenerateTokenForUserAsync(User user);

        Task<User> RegisterUserAsync(RegisterCommand command);

        Task<(string Token, string RefreshToken)> RefreshToken(string token, string refreshToken);
    }
}
