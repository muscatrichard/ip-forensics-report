namespace IPForensics.Core.Entities;

public sealed class ReputationMetrics
{
    public int AbuseConfidenceScore { get; set; }
    public int TotalReports { get; set; }
    public DateTimeOffset? LastReportedAt { get; set; }
}
