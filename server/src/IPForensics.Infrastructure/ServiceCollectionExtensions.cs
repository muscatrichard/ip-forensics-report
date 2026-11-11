using System.Net.Http.Headers;
using IPForensics.Core.Interfaces.External;
using IPForensics.Core.Interfaces.Repositories;
using IPForensics.Core.Interfaces.Security;
using IPForensics.Infrastructure.Data;
using IPForensics.Infrastructure.Data.Repositories;
using IPForensics.Infrastructure.ExternalServices;
using IPForensics.Infrastructure.Options;
using IPForensics.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IPForensics.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddOptions<EncryptionOptions>()
            .Bind(configuration.GetSection(EncryptionOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<AbuseIpDbOptions>()
            .Bind(configuration.GetSection(AbuseIpDbOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddOptions<GeoIpOptions>()
            .Bind(configuration.GetSection(GeoIpOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHttpClient<IGeoIpClient, GeoIpClient>((provider, client) =>
        {
            var options = provider.GetRequiredService<IOptions<GeoIpOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        services.AddHttpClient<IAbuseIpDbClient, AbuseIpDbClient>((provider, client) =>
        {
            var options = provider.GetRequiredService<IOptions<AbuseIpDbOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.Add("Key", options.ApiKey);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });

        services.AddSingleton<DbConnectionFactory>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddSingleton<IReportEncryptor, ReportEncryptor>();

        return services;
    }
}
