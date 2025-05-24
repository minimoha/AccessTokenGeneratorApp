using System.ComponentModel.DataAnnotations;
namespace AccessTokenGeneratorApp.Entities;

public class User
{
    [Key]
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
