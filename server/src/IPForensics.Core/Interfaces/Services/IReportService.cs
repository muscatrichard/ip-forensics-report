using IPForensics.Core.Dtos;
using IPForensics.Core.Dtos.Reports;

namespace IPForensics.Core.Interfaces.Services;

public interface IReportService
{
    Task<ReportDto> GenerateAsync(Guid userId, string ipAddress, CancellationToken cancellationToken);
    
    Task<ReportDto> GetAsync(Guid userId, Guid reportId, CancellationToken cancellationToken);

    Task<PagedResult<ReportSummaryDto>> ListAsync(
        Guid userId, int page, int pageSize, CancellationToken cancellationToken);
}
