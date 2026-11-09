using System.ComponentModel.DataAnnotations;

namespace IPForensics.Core.Dtos.Auth;

public sealed class LoginRequest
{
    [Required, StringLength(254)]
    public string Email { get; init; } = string.Empty;
    
    [Required, StringLength(128)]
    public string Password { get; init; } = string.Empty;
}
