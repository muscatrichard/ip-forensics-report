using IPForensics.Core.Dtos;
using IPForensics.Core.Dtos.Reports;
using IPForensics.Core.Entities;
using IPForensics.Core.Exceptions;
using IPForensics.Core.Interfaces.Repositories;
using IPForensics.Core.Interfaces.Security;
using IPForensics.Core.Interfaces.Services;

namespace IPForensics.Core.Services;

public sealed class ReportService : IReportService
{
    private readonly IIpLookupService _lookup;
    private readonly IReportEncryptor _encryptor;
    private readonly IReportRepository _reports;

    public ReportService(IIpLookupService lookup, IReportEncryptor encryptor, IReportRepository reports)
    {
        _lookup = lookup;
        _encryptor = encryptor;
        _reports = reports;
    }

    public async Task<ReportDto> GenerateAsync(Guid userId, string ipAddress, CancellationToken cancellationToken)
    {
        var report = await _lookup.LookupAsync(ipAddress, cancellationToken);

        var stored = new StoredReport
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            Payload = _encryptor.Encrypt(report, userId),
            CreatedAtUtc = DateTime.UtcNow
        };

        await _reports.AddAsync(stored, cancellationToken);

        return ToDto(stored, report);
    }

    public async Task<ReportDto> GetAsync(Guid userId, Guid reportId, CancellationToken cancellationToken)
    {
        var stored = await _reports.GetByIdAsync(userId, reportId, cancellationToken)
            ?? throw new ReportNotFoundException();

        return ToDto(stored, _encryptor.Decrypt(stored.Payload, userId));
    }

    public async Task<PagedResult<ReportSummaryDto>> ListAsync(
        Guid userId, int page, int pageSize, CancellationToken cancellationToken)
    {
        var size = Math.Min(pageSize, ReportListQuery.MaxPageSize);

        var (stored, totalCount) = await _reports.ListAsync(userId, (page - 1) * size, size, cancellationToken);

        var items = stored
            .Select(row =>
            {
                var report = _encryptor.Decrypt(row.Payload, userId);
                return new ReportSummaryDto
                {
                    Id = row.Id,
                    CreatedAtUtc = row.CreatedAtUtc,
                    IpAddress = report.IpAddress,
                    AbuseConfidenceScore = report.Reputation.AbuseConfidenceScore,
                    Country = report.Geolocation.Country
                };
            })
            .ToList();

        return new PagedResult<ReportSummaryDto>
        {
            Items = items,
            Page = page,
            PageSize = size,
            TotalCount = totalCount
        };
    }

    private static ReportDto ToDto(StoredReport stored, IPForensicsReport report) => new()
    {
        Id = stored.Id,
        CreatedAtUtc = stored.CreatedAtUtc,
        IpAddress = report.IpAddress,
        Reputation = report.Reputation,
        Geolocation = report.Geolocation,
        Network = report.Network
    };
}
