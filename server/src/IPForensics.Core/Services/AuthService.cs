using IPForensics.Core.Dtos.Auth;
using IPForensics.Core.Entities;
using IPForensics.Core.Exceptions;
using IPForensics.Core.Interfaces.Repositories;
using IPForensics.Core.Interfaces.Security;
using IPForensics.Core.Interfaces.Services;

namespace IPForensics.Core.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(
        IUserRepository users,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<UserDto> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Id = Guid.CreateVersion7(),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = NormalizeEmail(request.Email),
            PasswordHash = _passwordHasher.Hash(request.Password),
            CreatedAtUtc = DateTime.UtcNow
        };
        
        if (!await _users.TryAddAsync(user, cancellationToken))
        {
            throw new DuplicateEmailException();
        }

        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _users.FindByEmailAsync(NormalizeEmail(request.Email), cancellationToken);
        
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new InvalidCredentialsException();
        }

        return _tokenService.CreateToken(user);
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }
}
