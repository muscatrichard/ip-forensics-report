using IPForensics.Core.Entities;

namespace IPForensics.Core.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken);
    Task<bool> TryAddAsync(User user, CancellationToken cancellationToken);
}
