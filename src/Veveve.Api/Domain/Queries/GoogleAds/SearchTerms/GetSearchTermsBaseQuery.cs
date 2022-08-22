using Google.Ads.Gax.Lib;
using Google.Ads.GoogleAds.Config;
using Google.Ads.GoogleAds.Lib;
using Google.Ads.GoogleAds.V11.Services;
using MediatR;
using Veveve.Api.Domain.Services;

namespace Veveve.Api.Domain.Commands.GoogleAds;

public static class GetSearchTermBaseQuery
{
    public record Query(string GaQuery, string CustomerId) : IRequest<IEnumerable<SearchTermsDto>>;

    public class Handler : IRequestHandler<Query, IEnumerable<SearchTermsDto>>
    {
        private readonly GoogleAdsClient _client;

        public Handler(AdsClient<GoogleAdsConfig> client)
        {
            _client = (GoogleAdsClient)client;
        }

        public async Task<IEnumerable<SearchTermsDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            // "dynamic_search_ads_search_term_ivew.has_negative_keyword" and "metrics.cost" have been cut from the sql, because they are not selectable with the given FROM clause
            // see: https://developers.google.com/google-ads/api/fields/v11/dynamic_search_ads_search_term_view_query_builder for more info

            var gaRequest = new SearchGoogleAdsRequest()
            {
                CustomerId = request.CustomerId,
                Query = request.GaQuery
            };

            var googleAdsService = _client.GetService(Google.Ads.GoogleAds.Services.V11.GoogleAdsService);
            var apiResult = googleAdsService.SearchAsync(request: gaRequest);
            var searchTermList = new List<SearchTermsDto>();

            await foreach (var row in apiResult)
            {
                var dto = new SearchTermsDto();
                dto.AdGroupId = row.AdGroup.Id.ToString();
                dto.Clicks = row.Metrics.Clicks;
                dto.AdGroupName = row.AdGroup.Name;
                dto.CampaignName = row.Campaign.Name;
                dto.CampaignId = row.Campaign.Id.ToString();
                dto.ConversionValue = row.Metrics.ConversionsValue;
                dto.Conversions = row.Metrics.Conversions;
                if (request.GaQuery.Contains("FROM dynamic_search_ads_search_term_view"))
                {
                    dto.SearchTerm = row.DynamicSearchAdsSearchTermView.SearchTerm;
                }
                else
                {
                    dto.SearchTerm = row.SearchTermView.SearchTerm;
                }
                dto.Impressions = row.Metrics.Impressions;
                searchTermList.Add(dto);
            }

            return searchTermList;
        }
    }
}
