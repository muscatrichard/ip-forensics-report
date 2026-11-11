namespace IPForensics.Core.Entities;

public sealed class GeoIpResult
{
    public string Continent { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public bool Mobile { get; set; }
    public bool Proxy { get; set; }
    public bool Hosting { get; set; }
}
