using System.ComponentModel.DataAnnotations;

namespace IPForensics.Infrastructure.Options;

public sealed class AbuseIpDbOptions
{
    public const string SectionName = "AbuseIpDb";

    [Required, Url]
    public string BaseUrl { get; set; } = string.Empty;

    [Required(ErrorMessage = "AbuseIpDb:ApiKey is not set.")]
    public string ApiKey { get; set; } = string.Empty;
    
    [Range(1, 365)]
    public int MaxAgeInDays { get; set; } = 90;
}
