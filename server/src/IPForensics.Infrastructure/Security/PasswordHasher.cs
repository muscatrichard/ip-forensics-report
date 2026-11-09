using IPForensics.Core.Entities;
using IPForensics.Core.Interfaces.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace IPForensics.Infrastructure.Security;

public sealed class PasswordHasher : IPasswordHasher
{
    
    private const int IterationCount = 220_000;
    
    private static readonly User Placeholder = new();

    private readonly PasswordHasher<User> _hasher = new(
        new OptionsWrapper<PasswordHasherOptions>(
            new PasswordHasherOptions { IterationCount = IterationCount }));

    public string Hash(string password)
    {
        return _hasher.HashPassword(Placeholder, password);
    }

    public bool Verify(string password, string passwordHash)
    {
        var result = _hasher.VerifyHashedPassword(Placeholder, passwordHash, password);
        return result != PasswordVerificationResult.Failed;
    }
}
