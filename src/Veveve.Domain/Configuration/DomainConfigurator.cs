using Google.Ads.Gax.Config;
using Google.Ads.Gax.Lib;
using Google.Ads.GoogleAds.Config;
using Google.Ads.GoogleAds.Lib;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SendGrid;
using Veveve.Domain.Database;
using Veveve.Domain.Models.Options;
using Veveve.Domain.PipelineBehaviours;
using Veveve.Domain.Services;

namespace Veveve.Domain.Configuration;

public static class DomainConfigurator
{
    public static IServiceCollection AddDomain(this IServiceCollection services, IConfiguration configuration)
    {
        BindOptions(services, configuration);

        services
            .AddScoped<IPasswordService, PasswordService>()
            .AddScoped<ISendGridClientFacade, SendGridClientFacade>()
            .AddScoped<ISendGridClient, SendGridClient>(serviceProvider =>
            {
                var sendGridOptions = serviceProvider.GetRequiredService<IOptions<SendGridOptions>>().Value;
                return new SendGridClient(sendGridOptions.ApiKey);
            })
            .AddDbContext<AppDbContext>(opts =>
            {
                opts.UseNpgsql(configuration.GetConnectionString(nameof(ConnectionStringOptions.DbConnection)));
            })
            .AddMediatR(typeof(Veveve.Domain.Database.AppDbContext).Assembly) // use any type from Veveve.Domain
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
            // ====== Google Ads Client Setup ======
            .AddScoped<AdsClient<GoogleAdsConfig>, GoogleAdsClient>(serviceProvider =>
            {
                var googleAdsOptions = serviceProvider.GetRequiredService<IOptions<GoogleAdsApiOptions>>().Value;
                GoogleAdsConfig config = new GoogleAdsConfig()
                {
                    DeveloperToken = googleAdsOptions.DeveloperToken,
                    LoginCustomerId = googleAdsOptions.CustomerId,
                    OAuth2Mode = OAuth2Flow.APPLICATION,
                    OAuth2ClientId = googleAdsOptions.OAuth2ClientId,
                    OAuth2ClientSecret = googleAdsOptions.OAuth2ClientSecret,
                    OAuth2RefreshToken = googleAdsOptions.OAuth2RefreshToken
                };
                return new GoogleAdsClient(config);
            });
        // ====== Google Ads Client Setup ======;


        return services;
    }

    private static void BindOptions(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DefaultAdminDataOptions>(configuration.GetSection(DefaultAdminDataOptions.SectionName));
        services.Configure<ConnectionStringOptions>(configuration.GetSection(ConnectionStringOptions.SectionName));
        services.Configure<SendGridOptions>(configuration.GetSection(SendGridOptions.SectionName));
        services.Configure<AuthorizationOptions>(configuration.GetSection(AuthorizationOptions.SectionName));
        services.Configure<GoogleAdsApiOptions>(configuration.GetSection(GoogleAdsApiOptions.SectionName));
    }
}