using IPForensics.Core.Entities;

namespace IPForensics.Core.Interfaces.External;

public interface IAbuseIpDbClient
{
    Task<AbuseIpDbResult> LookupAsync(string ipAddress, CancellationToken cancellationToken);
}
