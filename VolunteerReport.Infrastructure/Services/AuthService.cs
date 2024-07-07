
using EnRoute.Common.Configuration;
using EnRoute.Domain.Models;
using EnRoute.Infrastructure.Commands;
using EnRoute.Infrastructure.Constants;
using EnRoute.Infrastructure.Extentions;
using EnRoute.Infrastructure.Services.Interfaces;
using EnRoute.Infrastructure.Strategies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;
using System.Security.Claims;
using VolunteerReport.Domain;
using VolunteerReport.Domain.Models;
using VolunteerReport.Infrastructure.Services.Interfaces;

namespace EnRoute.Infrastructure.Services
{
    /// <summary>
    /// Service handling authentication operations.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly JwtSettings jwtSettings;
        private readonly IJwtTokenService jwtTokenService;
        private readonly IGoogleJwtTokenParser googleJwtTokenParser;
        private readonly IRoleStrategyFactory roleStrategyFactory;

        public AuthService(ApplicationDbContext dbContext, JwtSettings jwtSettings, IJwtTokenService jwtTokenService, IGoogleJwtTokenParser googleJwtTokenParser, IRoleStrategyFactory roleStrategyFactory)
        {
            this.dbContext = dbContext;
            this.jwtSettings = jwtSettings;
            this.jwtTokenService = jwtTokenService;
            this.googleJwtTokenParser = googleJwtTokenParser;
            this.roleStrategyFactory = roleStrategyFactory;
        }

        /// <summary>
        /// Generates a token for the given user.
        /// </summary>
        /// <param name="user">User entity for which the token is generated.</param>
        /// <returns>A tuple containing the generated token and its refresh token.</returns>
        public async Task<(string Token, string RefreshToken)> GenerateTokenForUserAsync(User user)
        {
            var authClaims = new List<Claim>
        {
            new(JwtClaims.Sub, user.Id.ToString()),
            new(JwtClaims.Email, user.Email!),
            new(JwtClaims.RegisterDate, user.RegisterDate.ToString("O")),
            new(JwtClaims.Name, user.Name)
        };

            authClaims.Add(new Claim(JwtClaims.Roles, string.Join<string>(",", new[] { user.Role })));

            var token = jwtTokenService.CreateToken(authClaims);
            var refreshToken = jwtTokenService.GenerateRefreshToken();


            var serializedToken = token.SerializeToString();

            var issuedToken = new IssuedToken
            {
                User = user,
                Token = serializedToken,
                RefreshToken = refreshToken,
                RefreshTokenExpirationTime = DateTime.UtcNow.AddDays(jwtSettings.RefreshTokenValidityInDays)
            };

            dbContext.IssuedTokens.Add(issuedToken);
            await dbContext.SaveChangesAsync();

            return (serializedToken, refreshToken);
        }

        /// <summary>
        /// Registers a new user with the provided registration information.
        /// </summary>
        /// <param name="command">Registration command containing user information.</param>
        /// <returns>The newly registered user.</returns>
        public async Task<User> RegisterUserAsync(RegisterCommand command)
        {
            var user = new User()
            {
                Email = command.Email,
                Name = command.Name,
                Role = command.Role
            };

            var isUserExists = await dbContext.Users.AnyAsync(u => u.Email == user.Email);

            if (isUserExists)
            {
                throw new Exception($"User with the same email ({user.Email}) already exists.");
            }

            var newUser = await dbContext.Users.AddAsync(user);


            if (user == null)
            {
                throw new AuthenticationException("Invalid access token or refresh token");
            }
            try
            {
                await dbContext.SaveChangesAsync();
                var strategy = roleStrategyFactory.CreateStrategy(command.Role.ToLower());
                await strategy.ExecuteRoleSpecificActionAsync(newUser.Entity, command, dbContext);

                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                dbContext.Users.Remove(newUser.Entity);
                await dbContext.SaveChangesAsync();
                throw;
            }

            return newUser.Entity;
        }

        /// <summary>
        /// Refreshes the provided JWT token using the refresh token.
        /// </summary>
        /// <param name="token">Expired JWT token.</param>
        /// <param name="refreshToken">Refresh token.</param>
        /// <returns>A tuple containing the new token and refresh token.</returns>
        public async Task<(string Token, string RefreshToken)> RefreshToken(string token, string refreshToken)
        {
            var principal = this.jwtTokenService.GetPrincipalFromToken(token);
            if (principal == null)
            {
                throw new AuthenticationException("Invalid access token or refresh token");
            }

            var userId = Guid.Parse(principal.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value);

            var user = await dbContext.Users
                .Include(c => c.IssuedTokens)
                .Where(u => u.Id == userId &&
                            u.IssuedTokens.Any(t =>
                                t.RefreshToken == refreshToken && DateTime.UtcNow <= t.RefreshTokenExpirationTime))
                .FirstOrDefaultAsync();

            var currentToken = user.IssuedTokens.First(c => c.RefreshToken == refreshToken);
            dbContext.IssuedTokens.Remove(currentToken);

            return await GenerateTokenForUserAsync(user);
        }
    }
}
