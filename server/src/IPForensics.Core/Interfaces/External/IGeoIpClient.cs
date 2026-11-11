using IPForensics.Core.Entities;

namespace IPForensics.Core.Interfaces.External;

public interface IGeoIpClient
{
    Task<GeoIpResult> LookupAsync(string ipAddress, CancellationToken cancellationToken);
}
