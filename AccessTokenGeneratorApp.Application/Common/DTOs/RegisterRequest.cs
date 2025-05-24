namespace AccessTokenGeneratorApp.Application.Common.DTOs;

public class RegisterRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}


public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"\d").WithMessage("Password must contain at least one number.")
            .Matches(@"[\!\@\#\$\%\^\&\*\(\)\-\+]").WithMessage("Password must contain at least one special character.")
            .Matches(@"^\S+$").WithMessage("Password must not contain spaces.");
    }
}


public class TokenGenerateRequest
{
    public int ExpiryInMinutes { get; set; }
}

public class TokenVerifyRequest
{
    public string Token { get; set; } = null!;
}
