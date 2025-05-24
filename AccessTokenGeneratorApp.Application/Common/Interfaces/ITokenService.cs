namespace AccessTokenGeneratorApp.Application.Common.Interfaces;

public interface ITokenService
{
    Task<Result> GenerateTokenAsync(int expiryHours);
    Task<Result> VerifyTokenAsync(string token);
}