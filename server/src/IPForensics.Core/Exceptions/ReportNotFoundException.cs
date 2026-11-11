namespace IPForensics.Core.Exceptions;

public sealed class ReportNotFoundException : Exception
{
    public ReportNotFoundException()
        : base("Report not found.")
    {
    }
}
