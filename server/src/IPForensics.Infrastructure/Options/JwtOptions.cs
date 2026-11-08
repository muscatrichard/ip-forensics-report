using System.ComponentModel.DataAnnotations;

namespace IPForensics.Infrastructure.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required(ErrorMessage = "Jwt:Issuer is not set.")]
    public string Issuer { get; set; } = string.Empty;

    [Required(ErrorMessage = "Jwt:Audience is not set.")]
    public string Audience { get; set; } = string.Empty;

    [Range(1, 1440, ErrorMessage = "Jwt:ExpiryMinutes must be between 1 and 1440.")]
    public int ExpiryMinutes { get; set; } = 60;

    [Required(ErrorMessage = "Jwt:Key is not set. See the README for the user-secrets command.")]
    [MinLength(32, ErrorMessage = "Jwt:Key must be at least 32 characters.")]
    public string Key { get; set; } = string.Empty;
}
