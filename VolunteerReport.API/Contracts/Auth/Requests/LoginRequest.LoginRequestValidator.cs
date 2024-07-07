using FluentValidation;

namespace EnRoute.API.Contracts.Auth.Requests;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(c => c.googleOAuthJwt).NotEmpty();
    }
}