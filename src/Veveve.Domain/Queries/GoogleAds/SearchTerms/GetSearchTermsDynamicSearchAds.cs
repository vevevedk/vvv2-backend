using Google.Ads.Gax.Lib;
using Google.Ads.GoogleAds.Config;
using Google.Ads.GoogleAds.Lib;
using Google.Ads.GoogleAds.V11.Services;
using MediatR;
using Veveve.Domain.Services;

namespace Veveve.Domain.Commands.GoogleAds;

public static class GetSearchTermsDynamicSearchAds
{
    public record Query(string CustomerId, int LookbackDays) : IRequest<IEnumerable<SearchTermsDto>>;

    public class Handler : IRequestHandler<Query, IEnumerable<SearchTermsDto>>
    {
        private readonly GoogleAdsClient _client;

        public Handler(AdsClient<GoogleAdsConfig> client)
        {
            _client = (GoogleAdsClient)client;
        }

        public async Task<IEnumerable<SearchTermsDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            //var res2 = await _googleAdsClientFacade.GetSearchTerms(request.CustomerId, request.LookbackDays); // maybe I want the dto instead?

            // "dynamic_search_ads_search_term_ivew.has_negative_keyword" and "metrics.cost" have been cut from the sql, because they are not selectable with the given FROM clause
            // see: https://developers.google.com/google-ads/api/fields/v11/dynamic_search_ads_search_term_view_query_builder for more info
            var today = DateTime.Today.Date;
            var lookbackDate = today.AddDays(-request.LookbackDays).Date;
            var query = @$"SELECT
                                dynamic_search_ads_search_term_view.search_term,
                                dynamic_search_ads_search_term_view.has_matching_keyword,
                                ad_group.id,
                                ad_group.name,
                                campaign.id,
                                campaign.name,
                                metrics.impressions,
                                metrics.clicks,
                                metrics.conversions,
                                metrics.conversions_value
                            FROM dynamic_search_ads_search_term_view
                            WHERE campaign.status != 'REMOVED'
                                AND ad_group.status != 'REMOVED'
                                AND segments.date > '{lookbackDate.ToString("yyyy-MM-dd")}'
                                AND segments.date <= '{today.ToString("yyyy-MM-dd")}'
                            ORDER BY metrics.clicks DESC
                            LIMIT 100";

            var gaRequest = new SearchGoogleAdsRequest()
            {
                CustomerId = request.CustomerId,
                Query = query
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
                dto.SearchTerm = row.DynamicSearchAdsSearchTermView.SearchTerm;
                dto.Impressions = row.Metrics.Impressions;
                searchTermList.Add(dto);
            }

            return searchTermList;
            // TODO join the results together
        }
    }
}
