using System.ComponentModel.DataAnnotations;

namespace IPForensics.Core.Dtos.Reports;

public sealed class ReportListQuery
{
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 100;
    
    [Range(1, 100_000)]
    public int Page { get; set; } = 1;
    
    [Range(1, int.MaxValue)]
    public int PageSize { get; set; } = DefaultPageSize;
}
