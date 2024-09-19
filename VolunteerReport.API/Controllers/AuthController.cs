using AutoMapper;
using EnRoute.API.Contracts.Auth.Requests;
using EnRoute.API.Contracts.Auth.Responses;
using EnRoute.Common.Configuration;
using EnRoute.Domain;
using EnRoute.Domain.Models;
using EnRoute.Infrastructure.Commands;
using EnRoute.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VolunteerReport.Domain.Models;
using VolunteerReport.Domain;
using VolunteerReport.Infrastructure.Services.Interfaces;
using EnRoute.Infrastructure.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using EnRoute.Infrastructure.Extentions;
using Microsoft.EntityFrameworkCore;
using VolunteerReport.Common.Constants;
using VolunteerReport.Common.Configuration;

namespace EnRoute.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly IMapper mapper;
        private readonly ApplicationDbContext dbContext;
        private readonly JwtSettings jwtSettings;
        private readonly AdminSettings adminSettings;
        private readonly IGoogleJwtTokenParser jwtTokenParser;
        private readonly IConfiguration configuration;

        public AuthController(
            IAuthService authService,
            IMapper mapper,
            ApplicationDbContext dbContext,
            JwtSettings jwtSettings,
            AdminSettings adminSettings,
            IGoogleJwtTokenParser jwtTokenParser,
            IConfiguration configuration)
        {
            this.authService = authService;
            this.mapper = mapper;
            this.dbContext = dbContext;
            this.jwtSettings = jwtSettings;
            this.adminSettings = adminSettings;
            this.jwtTokenParser = jwtTokenParser;
            this.configuration = configuration;
        }

        [HttpPost]
        [Route("login/user")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var claims = await jwtTokenParser.GetPayloadFromToken(request.googleOAuthJwt);

            var name = claims.Name;
            var email = claims.Email;
            var photoUrl = claims.Picture;

            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user is null)
            {
                var registerCommand = new RegisterCommand
                {
                    Email = email,
                    Name = name,
                    AvatarUrl = photoUrl,
                    Role = adminSettings.Emails.Any(e => e.Equals(email,StringComparison.InvariantCultureIgnoreCase)) ? 
                                UserRoles.Administrator :
                                UserRoles.User
                };
                user = await authService.RegisterUserAsync(registerCommand);
            }

            var (token, refreshToken) = await authService.GenerateTokenForUserAsync(user);

            var response = new LoginResponse(token, refreshToken);
            return Ok(response);
        }

        [HttpPost]
        [Route("login/volunteer")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> LoginVolunteer([FromBody] LoginRequest request)
        {
            var claims = await jwtTokenParser.GetPayloadFromToken(request.googleOAuthJwt);

            var name = claims.Name;
            var email = claims.Email;
            var photoUrl = claims.Picture;

            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user is null)
            {
                var registerCommand = new RegisterCommand
                {
                    Email = email,
                    Name = name,
                    AvatarUrl = photoUrl,
                    Role = UserRoles.Volunteer
                };
                user = await authService.RegisterUserAsync(registerCommand);
            }

            var (token, refreshToken) = await authService.GenerateTokenForUserAsync(user);

            var response = new LoginResponse(token, refreshToken);
            return Ok(response);
        }


        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            var (token, refreshToken) = await authService.RefreshToken(refreshTokenRequest.Token, refreshTokenRequest.RefreshToken);

            var response = new RefreshTokenResponse(token, refreshToken);

            return Ok(response);
        }
    }
}
