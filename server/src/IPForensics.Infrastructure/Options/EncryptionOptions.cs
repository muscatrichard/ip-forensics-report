using System.ComponentModel.DataAnnotations;

namespace IPForensics.Infrastructure.Options;

public sealed class EncryptionOptions : IValidatableObject
{
    public const string SectionName = "Encryption";
    public const int KeySizeBytes = 32;

    [Required(ErrorMessage = "Encryption:Key is not set.")]
    public string Key { get; set; } = string.Empty;


    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(Key))
        {
            yield break; 
        }

        var buffer = new byte[Key.Length];
        if (!Convert.TryFromBase64String(Key, buffer, out var written) || written != KeySizeBytes)
        {
            yield return new ValidationResult(
                "Encryption:Key must be a Base64 string that decodes to 32 bytes (generate one with: openssl rand -base64 32).",
                new[] { nameof(Key) });
        }
    }
}
