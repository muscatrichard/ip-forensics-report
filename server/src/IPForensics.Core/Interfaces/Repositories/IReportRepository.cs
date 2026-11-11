using IPForensics.Core.Entities;

namespace IPForensics.Core.Interfaces.Repositories;

public interface IReportRepository
{
    Task AddAsync(StoredReport report, CancellationToken cancellationToken);
    
    Task<StoredReport?> GetByIdAsync(Guid userId, Guid reportId, CancellationToken cancellationToken);
    
    Task<(IReadOnlyList<StoredReport> Items, int TotalCount)> ListAsync(
        Guid userId, int skip, int take, CancellationToken cancellationToken);
}
