namespace IPForensics.Core.Entities;

public sealed class AbuseIpDbResult
{
    public int AbuseConfidenceScore { get; set; }
    public int TotalReports { get; set; }
    public DateTimeOffset? LastReportedAt { get; set; }
    public bool IsTor { get; set; }
}
