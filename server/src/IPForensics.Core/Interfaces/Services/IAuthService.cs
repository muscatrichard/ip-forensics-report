using IPForensics.Core.Dtos.Auth;

namespace IPForensics.Core.Interfaces.Services;

public interface IAuthService
{
    /// <exception cref="Exceptions.DuplicateEmailException">The email is already registered.</exception>
    Task<UserDto> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);

    /// <exception cref="Exceptions.InvalidCredentialsException">The email or password is wrong.</exception>
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
}
