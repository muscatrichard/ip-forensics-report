namespace IPForensics.Core.Dtos.Reports;

public sealed class ReportSummaryDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public int AbuseConfidenceScore { get; set; }
    public string Country { get; set; } = string.Empty;
}
