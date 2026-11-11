using IPForensics.Core.Entities;

namespace IPForensics.Core.Dtos.Reports;

public sealed class ReportDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public ReputationMetrics Reputation { get; set; } = new();
    public GeolocationData Geolocation { get; set; } = new();
    public NetworkAttributes Network { get; set; } = new();
}
