namespace AccessTokenGeneratorApp.Application.Common.Interfaces;

public interface IAuthService
{
    Task<Result> RegisterAsync(RegisterRequest request);
    Task<Result> LoginAsync(LoginRequest request);
}
