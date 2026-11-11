using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using IPForensics.Core.Entities;
using IPForensics.Core.Exceptions;
using IPForensics.Core.Interfaces.External;
using Microsoft.Extensions.Logging;

namespace IPForensics.Infrastructure.ExternalServices;

public sealed class GeoIpClient : IGeoIpClient
{
    private const string Fields = "status,message,continent,country,regionName,city,mobile,proxy,hosting";

    private readonly HttpClient _http;
    private readonly ILogger<GeoIpClient> _logger;

    public GeoIpClient(HttpClient http, ILogger<GeoIpClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<GeoIpResult> LookupAsync(string ipAddress, CancellationToken cancellationToken)
    {
        var url = $"{Uri.EscapeDataString(ipAddress)}?fields={Fields}";

        try
        {
            using var response = await _http.GetAsync(url, cancellationToken);

            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                throw new ExternalServiceException(
                    "The geolocation service is limiting requests right now. Please try again in a minute.");
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("ip-api returned HTTP {StatusCode}", (int)response.StatusCode);
                throw new ExternalServiceException("The geolocation service is unavailable. Please try again later.");
            }

            var body = await response.Content.ReadFromJsonAsync<IpApiResponse>(cancellationToken);
            
            if (body is null || body.Status != "success")
            {
                _logger.LogWarning("ip-api lookup failed: {Message}", body?.Message);
                throw new ExternalServiceException("The geolocation service could not look up that address.");
            }

            return new GeoIpResult
            {
                Continent = body.Continent ?? string.Empty,
                Country = body.Country ?? string.Empty,
                Region = body.RegionName ?? string.Empty,
                City = body.City ?? string.Empty,
                Mobile = body.Mobile,
                Proxy = body.Proxy,
                Hosting = body.Hosting
            };
        }
        catch (Exception ex) when (
            (ex is HttpRequestException or JsonException or TaskCanceledException)
            && !cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(ex, "ip-api request failed");
            throw new ExternalServiceException(
                "The geolocation service could not be reached. Please try again later.", ex);
        }
    }
    
    private sealed class IpApiResponse
    {
        public string? Status { get; set; }
        public string? Message { get; set; }
        public string? Continent { get; set; }
        public string? Country { get; set; }
        public string? RegionName { get; set; }
        public string? City { get; set; }
        public bool Mobile { get; set; }
        public bool Proxy { get; set; }
        public bool Hosting { get; set; }
    }
}
