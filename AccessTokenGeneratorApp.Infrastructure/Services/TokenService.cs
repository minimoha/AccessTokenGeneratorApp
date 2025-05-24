namespace AccessTokenGeneratorApp.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContext;

    public TokenService(IApplicationDbContext context, IHttpContextAccessor httpContext)
    {
        _context = context;
        _httpContext = httpContext;
    }

    public async Task<Result> GenerateTokenAsync(int expiryMinutes)
    {
        var userId = GetCurrentUserId();

        if (!userId.HasValue) { return Result.Failure("Unknown User."); }

        if (expiryMinutes < 1 || expiryMinutes > 4320)
            return Result.Failure("Token expiry must be between 1 and 4320 minutes (max 3 days).");

        var token = Guid.NewGuid().ToString("N")[..6].ToUpper();
        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var accessToken = new AccessToken
        {
            Token = token,
            ExpiresAt = expiresAt,
            UserId = userId.Value,
            CreatedAt = DateTime.Now

        };

        _context.AccessTokens.Add(accessToken);
        await _context.SaveChangesAsync();

        return Result.Success( new { token });
    }

    public async Task<Result> VerifyTokenAsync(string token)
    {
        var userId = GetCurrentUserId();

        if (!userId.HasValue) { return Result.Failure("Unknown User."); }

        var matchingTokens = await _context.AccessTokens
                        .Where(t => t.UserId == userId && t.Token == token)
                        .OrderByDescending(t => t.ExpiresAt)
                        .ToListAsync();

        var accessToken = matchingTokens.FirstOrDefault();

        if (accessToken == null)
            return Result.Failure(new { Valid = false, Reason = "Token does not exist." });

        if (accessToken.ExpiresAt < DateTime.UtcNow)
            return Result.Success(new { IsValid = false, Reason = "Token has expired." });

        return Result.Success(new {IsValid = true, Reason = "Token is valid."});
    }
    
    int? GetCurrentUserId()
    {
        var username = _httpContext.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return !string.IsNullOrEmpty(username) ? Convert.ToInt32(username) : null;
    }
}