namespace AccessTokenGeneratorApp.Infrastructure.Services;

public class AuthService(IApplicationDbContext context, IConfiguration configuration, IPasswordHasher<User> passwordHasher, IEmailSender emailSender) : IAuthService
{

    public async Task<Result> RegisterAsync(RegisterRequest request)
    {
        #region Validate inputs

        var validator = new RegisterRequestValidator();
        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            Log.Error("Login Validation Errors: {errors}", string.Join(" | ", errorMessages));
            return Result.Error("Validation failed", errorMessages);
        }

        #endregion

        if (context.Users.Any(u => u.Email == request.Email))
            return Result.Failure("Email already exists");

        var user = new User { Email = request.Email };
        var passwordHash = passwordHasher.HashPassword(user, request.Password);
        user.PasswordHash = passwordHash;
        user.CreatedAt = DateTime.Now;

        context.Users.Add(user);
        await context.SaveChangesAsync();

        await emailSender.SendEmailAsync(request.Email, "Registration Confirmation", "Hello User, \nYour registration was successful! You can now log in and start using your account.");

        return Result.Success("User successfully created.");
    }

    public async Task<Result> LoginAsync(LoginRequest request)
    {
        #region Validate inputs

        var validator = new LoginRequestValidator();
        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            Log.Error("Login Validation Errors: {errors}", string.Join(" | ", errorMessages));
            return Result.Error("Validation failed", errorMessages);
        }

        #endregion

        var user = await context.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

        if (user is null)
        {
            return Result.Failure("Invalid email or password");
        }

        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (result != PasswordVerificationResult.Success)
            return Result.Failure("Invalid email or password");

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[] 
            { 
                new(ClaimTypes.NameIdentifier, user.Id.ToString()), 
                new("username", request.Email) 
            }),
            Expires = DateTime.UtcNow.AddMinutes(10),
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Result.Success(new TokenDto { AccessToken = tokenHandler.WriteToken(token) });
    }
}
