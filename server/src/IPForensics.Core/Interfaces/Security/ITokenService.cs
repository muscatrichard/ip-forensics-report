using IPForensics.Core.Dtos.Auth;
using IPForensics.Core.Entities;

namespace IPForensics.Core.Interfaces.Security;

public interface ITokenService
{
    AuthResponse CreateToken(User user);
}
