using Google.Ads.Gax.Lib;
using Google.Ads.GoogleAds.Config;
using Google.Ads.GoogleAds.Lib;

namespace Veveve.Api.Domain.Services
{
    public class GoogleAdsClientFacade : IGoogleAdsClientFacade
    {
        private readonly ILogger<GoogleAdsClientFacade> _logger;
        private readonly AdsClient<GoogleAdsConfig> _client;

        public GoogleAdsClientFacade(
            ILogger<GoogleAdsClientFacade> logger,
            AdsClient<GoogleAdsConfig> client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task InitialTest()
        {
            var castedClient = (GoogleAdsClient) _client;
            
            _logger.Log(LogLevel.Information, "InitialTest");
            
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
