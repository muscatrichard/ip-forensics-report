using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using IPForensics.Core.Entities;
using IPForensics.Core.Exceptions;
using IPForensics.Core.Interfaces.External;
using IPForensics.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IPForensics.Infrastructure.ExternalServices;

public sealed class AbuseIpDbClient : IAbuseIpDbClient
{
    private readonly HttpClient _http;
    private readonly AbuseIpDbOptions _options;
    private readonly ILogger<AbuseIpDbClient> _logger;

    public AbuseIpDbClient(HttpClient http, IOptions<AbuseIpDbOptions> options, ILogger<AbuseIpDbClient> logger)
    {
        _http = http;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<AbuseIpDbResult> LookupAsync(string ipAddress, CancellationToken cancellationToken)
    {
        var url = $"check?ipAddress={Uri.EscapeDataString(ipAddress)}&maxAgeInDays={_options.MaxAgeInDays}";

        try
        {
            using var response = await _http.GetAsync(url, cancellationToken);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    break;

                case HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden:
                    _logger.LogError("AbuseIPDB rejected the configured API key (HTTP {StatusCode})", (int)response.StatusCode);
                    throw new ExternalServiceException("The reputation service is not configured correctly.");

                case HttpStatusCode.TooManyRequests:
                    throw new ExternalServiceException(
                        "The daily limit for the reputation service has been reached. Please try again later.");

                default:
                    _logger.LogWarning("AbuseIPDB returned HTTP {StatusCode}", (int)response.StatusCode);
                    throw new ExternalServiceException("The reputation service is unavailable. Please try again later.");
            }

            var body = await response.Content.ReadFromJsonAsync<CheckResponse>(cancellationToken);

            if (body?.Data is null)
            {
                _logger.LogWarning("AbuseIPDB returned an unexpected body");
                throw new ExternalServiceException("The reputation service returned an unexpected response.");
            }

            return new AbuseIpDbResult
            {
                AbuseConfidenceScore = body.Data.AbuseConfidenceScore,
                TotalReports = body.Data.TotalReports,
                LastReportedAt = body.Data.LastReportedAt,
                IsTor = body.Data.IsTor
            };
        }
        catch (Exception ex) when (
            (ex is HttpRequestException or JsonException or TaskCanceledException)
            && !cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(ex, "AbuseIPDB request failed");
            throw new ExternalServiceException(
                "The reputation service could not be reached. Please try again later.", ex);
        }
    }
    
    private sealed class CheckResponse
    {
        public CheckData? Data { get; set; }
    }

    private sealed class CheckData
    {
        public int AbuseConfidenceScore { get; set; }
        public int TotalReports { get; set; }
        public DateTimeOffset? LastReportedAt { get; set; }
        public bool IsTor { get; set; }
    }
}
