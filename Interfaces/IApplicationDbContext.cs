namespace AccessTokenGeneratorApp.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<AccessToken> AccessTokens { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
