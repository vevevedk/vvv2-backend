using Google.Ads.GoogleAds.Lib;
using Google.Ads.GoogleAds.V11.Services;
using Google.Api.Gax;

namespace Veveve.Api.Domain.Services
{
    public interface IGoogleAdsClientFacade
    {
        public Task<List<SearchTermsDto>> GetSearchTermsDynamicSearchAds(string customerId, int lookbackDays);
        public Task<PagedAsyncEnumerable<SearchGoogleAdsResponse, GoogleAdsRow>> GetSearchTerms(string customerId, int lookbackDays);
    }
}
