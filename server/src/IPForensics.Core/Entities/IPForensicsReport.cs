namespace IPForensics.Core.Entities;

public sealed class IPForensicsReport
{
    public string IpAddress { get; set; } = string.Empty;
    public ReputationMetrics Reputation { get; set; } = new();
    public GeolocationData Geolocation { get; set; } = new();
    public NetworkAttributes Network { get; set; } = new();
}
