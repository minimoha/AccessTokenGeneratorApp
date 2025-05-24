namespace AccessTokenGeneratorApp.Entities;

public class AccessToken
{
    public int Id { get; set; }
    public string Token { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public int UserId { get; set; }
}
