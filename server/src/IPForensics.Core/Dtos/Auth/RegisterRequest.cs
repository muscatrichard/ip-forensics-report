using System.ComponentModel.DataAnnotations;

namespace IPForensics.Core.Dtos.Auth;

public sealed class RegisterRequest
{
    [Required, StringLength(100)]
    public string FirstName { get; init; } = string.Empty;

    [Required, StringLength(100)]
    public string LastName { get; init; } = string.Empty;

    [Required, EmailAddress, StringLength(254)]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        MatchTimeoutInMilliseconds = 250,
        ErrorMessage = "Email must include a domain, for example name@example.com.")]
    public string Email { get; init; } = string.Empty;
    
    [Required, StringLength(128, MinimumLength = 8)]
    public string Password { get; init; } = string.Empty;
}
