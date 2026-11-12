using System.Security.Cryptography;
using IPForensics.Core.Dtos.Auth;
using IPForensics.Core.Entities;
using IPForensics.Core.Interfaces.External;
using IPForensics.Core.Interfaces.Repositories;
using IPForensics.Core.Interfaces.Security;
using IPForensics.Core.Interfaces.Services;
using IPForensics.Infrastructure.Options;
using IPForensics.Infrastructure.Security;
using Microsoft.Extensions.Options;

namespace IPForensics.Tests;

internal static class TestData
{
    public static IPForensicsReport Report(string ip = "8.8.8.8") => new()
    {
        IpAddress = ip,
        Reputation = new ReputationMetrics
        {
            AbuseConfidenceScore = 7,
            TotalReports = 188,
            LastReportedAt = new DateTimeOffset(2026, 11, 10, 18, 26, 57, TimeSpan.Zero)
        },
        Geolocation = new GeolocationData
        {
            Continent = "North America", Country = "United States", Region = "Virginia", City = "Ashburn"
        },
        Network = new NetworkAttributes { Mobile = false, Proxy = true, Hosting = true, Tor = false }
    };
    
    public static ReportEncryptor Encryptor() =>
        new(new OptionsWrapper<EncryptionOptions>(new EncryptionOptions
        {
            Key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
        }));
}

internal sealed class FakeGeoIpClient : IGeoIpClient
{
    public int Calls { get; private set; }
    public string? LastIp { get; private set; }
    public Exception? ThrowOnLookup { get; set; }

    public GeoIpResult Result { get; set; } = new()
    {
        Continent = "North America", Country = "United States", Region = "Virginia", City = "Ashburn",
        Mobile = false, Proxy = true, Hosting = true
    };

    public Task<GeoIpResult> LookupAsync(string ipAddress, CancellationToken cancellationToken)
    {
        Calls++;
        LastIp = ipAddress;
        return ThrowOnLookup is null
            ? Task.FromResult(Result)
            : Task.FromException<GeoIpResult>(ThrowOnLookup);
    }
}

internal sealed class FakeAbuseIpDbClient : IAbuseIpDbClient
{
    public int Calls { get; private set; }
    public string? LastIp { get; private set; }
    public Exception? ThrowOnLookup { get; set; }

    public AbuseIpDbResult Result { get; set; } = new()
    {
        AbuseConfidenceScore = 7,
        TotalReports = 188,
        LastReportedAt = new DateTimeOffset(2026, 11, 10, 18, 26, 57, TimeSpan.Zero),
        IsTor = true
    };

    public Task<AbuseIpDbResult> LookupAsync(string ipAddress, CancellationToken cancellationToken)
    {
        Calls++;
        LastIp = ipAddress;
        return ThrowOnLookup is null
            ? Task.FromResult(Result)
            : Task.FromException<AbuseIpDbResult>(ThrowOnLookup);
    }
}

internal sealed class FakeIpLookupService : IIpLookupService
{
    public Task<IPForensicsReport> LookupAsync(string ipAddress, CancellationToken cancellationToken) =>
        Task.FromResult(TestData.Report(ipAddress));
}

internal sealed class FakeUserRepository : IUserRepository
{
    private readonly Dictionary<string, User> _users = new();

    public IReadOnlyCollection<User> Users => _users.Values;

    public Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken) =>
        Task.FromResult(_users.GetValueOrDefault(email));

    public Task<bool> TryAddAsync(User user, CancellationToken cancellationToken) =>
        Task.FromResult(_users.TryAdd(user.Email, user));
}

internal sealed class FakeTokenService : ITokenService
{
    public AuthResponse CreateToken(User user) => new()
    {
        Token = $"token-for-{user.Id}",
        ExpiresAtUtc = DateTime.UtcNow.AddMinutes(60)
    };
}

internal sealed class FakeReportRepository : IReportRepository
{
    public List<StoredReport> Rows { get; } = new();
    public int LastSkip { get; private set; }
    public int LastTake { get; private set; }
    
    public bool IgnoreOwnerFilter { get; set; }

    public Task AddAsync(StoredReport report, CancellationToken cancellationToken)
    {
        Rows.Add(report);
        return Task.CompletedTask;
    }

    public Task<StoredReport?> GetByIdAsync(Guid userId, Guid reportId, CancellationToken cancellationToken) =>
        Task.FromResult(Rows.FirstOrDefault(r => r.Id == reportId && (IgnoreOwnerFilter || r.UserId == userId)));

    public Task<(IReadOnlyList<StoredReport> Items, int TotalCount)> ListAsync(
        Guid userId, int skip, int take, CancellationToken cancellationToken)
    {
        LastSkip = skip;
        LastTake = take;

        var mine = Rows.Where(r => r.UserId == userId).OrderByDescending(r => r.CreatedAtUtc).ToList();
        IReadOnlyList<StoredReport> page = mine.Skip(skip).Take(take).ToList();

        return Task.FromResult((page, mine.Count));
    }
}
