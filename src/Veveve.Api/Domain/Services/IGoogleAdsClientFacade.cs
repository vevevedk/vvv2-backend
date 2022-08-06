using Google.Ads.GoogleAds.Lib;

namespace Veveve.Api.Domain.Services
{
    public interface IGoogleAdsClientFacade
    {
        public abstract GoogleAdsClient InitializeClient();
    }
}
