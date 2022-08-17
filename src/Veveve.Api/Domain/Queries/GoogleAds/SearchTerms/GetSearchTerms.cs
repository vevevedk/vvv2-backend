// using Google.Ads.Gax.Lib;
// using Google.Ads.GoogleAds.Config;
// using Google.Ads.GoogleAds.Lib;
// using Google.Ads.GoogleAds.V11.Services;
// using MediatR;
// using Veveve.Api.Domain.Services;

// namespace Veveve.Api.Domain.Commands.GoogleAds;

// public static class GetSearchTerms
// {
//     public record Query(string CustomerId, int LookbackDays) : IRequest<PagedAsyncEnumerable<SearchGoogleAdsResponse, GoogleAdsRow>>;

//     public class Handler : IRequestHandler<Query, PagedAsyncEnumerable<SearchGoogleAdsResponse, GoogleAdsRow>>
//     {
//         private readonly AdsClient<GoogleAdsConfig> _client;

//         public Handler(IGoogleAdsClientFacade clientFacade,
//         AdsClient<GoogleAdsConfig> client)
//         {
//             _client = client;
//         }

//         public async Task<PagedAsyncEnumerable<SearchGoogleAdsResponse, GoogleAdsRow>> Handle(Query request, CancellationToken cancellationToken)
//         {
//             throw new NotImplementedException();
//             #region sample code
//             //try
//             //{
//             //    var castedClient = (GoogleAdsClient)_client;
//             //    var today = DateTime.Today;
//             //    var lookbackDate = today.AddDays(-lookbackDays);
//             //    var query = @$"SELECT
//             //                        search_term_view.search_term,
//             //                        search_term_view.status,
//             //                        ad_group.id,
//             //                        ad_group.name,
//             //                        campaign.id,
//             //                        campaign.name,
//             //                  metrics.impressions,
//             //                  metrics.clicks,
//             //                  metrics.cost,
//             //                  metrics.conversions,
//             //                  metricds.conversions_value
//             //                    FROM search_term_view
//             //                    WHERE campaign.status != \'REMOVED\'
//             //                        AND ad_group.status != \'REMOVED\'
//             //                        AND campaign.advertising_channel_type = \'SEARCH\'
//             //                        AND ad_group.type = \'SEARCH_STANDARD\'
//             //                        AND segments.date > '{lookbackDate.Date}'
//             //                        AND segments.date <= '{today.Date}'";

//             //    var request = new SearchGoogleAdsRequest()
//             //    {
//             //        CustomerId = customerId,
//             //        Query = query
//             //    };

//             //    var googleAdsService = castedClient.GetService(Google.Ads.GoogleAds.Services.V11.GoogleAdsService);
//             //    return Task.FromResult(googleAdsService.SearchAsync(request: request));

//             //}
//             //catch (Exception ex)
//             //{
//             //    _logger.Log(LogLevel.Error, ex.Message);
//             //    throw;
//             //}
//             #endregion
//         }
//     }
// }
