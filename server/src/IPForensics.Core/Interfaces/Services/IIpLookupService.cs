using IPForensics.Core.Entities;

namespace IPForensics.Core.Interfaces.Services;

public interface IIpLookupService
{
    Task<IPForensicsReport> LookupAsync(string ipAddress, CancellationToken cancellationToken);
}
