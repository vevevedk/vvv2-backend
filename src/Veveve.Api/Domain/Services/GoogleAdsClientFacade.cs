using Google.Ads.Gax.Config;
using Google.Ads.GoogleAds.Config;
using Google.Ads.GoogleAds.Lib;
using Google.Ads.GoogleAds.V10.Services;
using Microsoft.Extensions.Options;

namespace Veveve.Api.Domain.Services
{
    public class GoogleAdsClientFacade : IGoogleAdsClientFacade
    {
        private readonly ILogger<GoogleAdsClientFacade> _logger;
        private readonly GoogleAdsClient _client;
        private readonly GoogleAdsApiSettings _settings;

        public GoogleAdsClientFacade(
            ILogger<GoogleAdsClientFacade> logger,
            IOptions<GoogleAdsApiSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;
            _client = InitializeClient();
        }

        public GoogleAdsClient InitializeClient()
        {
            if (_client == null)
            {
                GoogleAdsConfig config = new GoogleAdsConfig()
                {
                    DeveloperToken = _settings.DeveloperToken,
                    OAuth2Mode = OAuth2Flow.SERVICE_ACCOUNT, //might have to be application
                    OAuth2ClientId = _settings.OAuth2ClientId,
                    OAuth2ClientSecret = _settings.OAuth2ClientSecret,
                    OAuth2RefreshToken = _settings.OAuth2RefreshToken
                };
                return new GoogleAdsClient(config);
            }
            return _client;

        }

        public void SomeCoolFeature()
        {
            //var request = new SearchGoogleAdsStreamRequest()
            //{
            //    CustomerId = "123",
            //    Query = "SELECT * FROM CAMPAIGNS"
            //};
            //var service = _client.GetService<GoogleAdsServiceClient>(IServiceProvider provider);
            //var resultStream = service.SearchStream(request);
            //var res = resultStream.GetResponseStream();
            //res.ToString();
        }
    }
}
