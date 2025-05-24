namespace AccessTokenGeneratorApp.Controllers;

[ApiController]
[Authorize]
[Route("token")]
public class TokenController : ControllerBase
{
    private readonly ITokenService _tokenService;
    public TokenController(ITokenService tokenService) => _tokenService = tokenService;

    [HttpPost("generate")]
    public async Task<IActionResult> Generate(TokenGenerateRequest request)
    {
        return Ok(await _tokenService.GenerateTokenAsync(request.ExpiryInMinutes));
    }

    [HttpPost("verify")]
    public async Task<IActionResult> Verify(TokenVerifyRequest request)
    {
        return Ok(await _tokenService.VerifyTokenAsync(request.Token));
    }
}