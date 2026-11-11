using System.ComponentModel.DataAnnotations;

namespace IPForensics.Core.Dtos.Reports;

public sealed class CreateReportRequest
{
    [Required, StringLength(45)]
    public string IpAddress { get; set; } = string.Empty;
}
