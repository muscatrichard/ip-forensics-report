using System.ComponentModel.DataAnnotations;

namespace IPForensics.Infrastructure.Options;

public sealed class GeoIpOptions
{
    public const string SectionName = "GeoIp";

    [Required, Url]
    public string BaseUrl { get; set; } = string.Empty;
}

