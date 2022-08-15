using Google.Ads.Gax.Lib;
using Google.Ads.GoogleAds.Config;
using Google.Ads.GoogleAds.Lib;
using Google.Ads.GoogleAds.V11.Errors;
using Google.Ads.GoogleAds.V11.Resources;
using Google.Ads.GoogleAds.V11.Services;
using Google.Api.Gax;

namespace Veveve.Api.Domain.Services;

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

    public async Task<List<SearchTermsDto>> GetSearchTermsDynamicSearchAds(string customerId, int lookbackDays)
    {
        try
        {
            // "dynamic_search_ads_search_term_ivew.has_negative_keyword" and "metrics.cost" have been cut from the sql, because they are not selectable with the given FROM clause
            // see: https://developers.google.com/google-ads/api/fields/v11/dynamic_search_ads_search_term_view_query_builder for more info
            var castedClient = (GoogleAdsClient)_client;
            var today = DateTime.Today.Date;
            var lookbackDate = today.AddDays(-lookbackDays).Date;
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

            var request = new SearchGoogleAdsRequest()
            {
                CustomerId = customerId,
                Query = query
            };

            var googleAdsService = castedClient.GetService(Google.Ads.GoogleAds.Services.V11.GoogleAdsService);
            var apiResult = googleAdsService.Search(request: request);
            var searchTermList = new List<SearchTermsDto>();

            foreach (var row in apiResult)
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

        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.Message);
            throw;
        }
    }

    public Task<PagedAsyncEnumerable<SearchGoogleAdsResponse, GoogleAdsRow>> GetSearchTerms(string customerId, int lookbackDays)
    {
        throw new NotImplementedException();
        #region sample code
        //try
        //{
        //    var castedClient = (GoogleAdsClient)_client;
        //    var today = DateTime.Today;
        //    var lookbackDate = today.AddDays(-lookbackDays);
        //    var query = @$"SELECT
        //                        search_term_view.search_term,
        //                        search_term_view.status,
        //                        ad_group.id,
        //                        ad_group.name,
        //                        campaign.id,
        //                        campaign.name,
        //                  metrics.impressions,
        //                  metrics.clicks,
        //                  metrics.cost,
        //                  metrics.conversions,
        //                  metricds.conversions_value
        //                    FROM search_term_view
        //                    WHERE campaign.status != \'REMOVED\'
        //                        AND ad_group.status != \'REMOVED\'
        //                        AND campaign.advertising_channel_type = \'SEARCH\'
        //                        AND ad_group.type = \'SEARCH_STANDARD\'
        //                        AND segments.date > '{lookbackDate.Date}'
        //                        AND segments.date <= '{today.Date}'";

        //    var request = new SearchGoogleAdsRequest()
        //    {
        //        CustomerId = customerId,
        //        Query = query
        //    };

        //    var googleAdsService = castedClient.GetService(Google.Ads.GoogleAds.Services.V11.GoogleAdsService);
        //    return Task.FromResult(googleAdsService.SearchAsync(request: request));

        //}
        //catch (Exception ex)
        //{
        //    _logger.Log(LogLevel.Error, ex.Message);
        //    throw;
        //}
        #endregion
    }
}
