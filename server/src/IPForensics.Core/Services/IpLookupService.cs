using IPForensics.Core.Entities;
using IPForensics.Core.Interfaces.External;
using IPForensics.Core.Interfaces.Services;

namespace IPForensics.Core.Services;

public sealed class IpLookupService : IIpLookupService
{
    private readonly IGeoIpClient _geoIp;
    private readonly IAbuseIpDbClient _abuseIpDb;

    public IpLookupService(IGeoIpClient geoIp, IAbuseIpDbClient abuseIpDb)
    {
        _geoIp = geoIp;
        _abuseIpDb = abuseIpDb;
    }

    public async Task<IPForensicsReport> LookupAsync(string ipAddress, CancellationToken cancellationToken)
    {
        var ip = IpAddressValidator.ValidatePublic(ipAddress);
        
        var geoTask = _geoIp.LookupAsync(ip, cancellationToken);
        var abuseTask = _abuseIpDb.LookupAsync(ip, cancellationToken);
        await Task.WhenAll(geoTask, abuseTask);

        var geo = await geoTask;
        var abuse = await abuseTask;

        return new IPForensicsReport
        {
            IpAddress = ip,
            Reputation = new ReputationMetrics
            {
                AbuseConfidenceScore = abuse.AbuseConfidenceScore,
                TotalReports = abuse.TotalReports,
                LastReportedAt = abuse.LastReportedAt
            },
            Geolocation = new GeolocationData
            {
                Continent = geo.Continent,
                Country = geo.Country,
                Region = geo.Region,
                City = geo.City
            },
            Network = new NetworkAttributes
            {
                Mobile = geo.Mobile,
                Proxy = geo.Proxy,
                Hosting = geo.Hosting,
                Tor = abuse.IsTor
            }
        };
    }
}
