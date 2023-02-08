using MediatR;
using Veveve.Domain.Services;

namespace Veveve.Domain.Queries.GoogleAds.SearchTerms;

public static class GetSearchTerms
{
    public record Query(string CustomerId, int LookbackDays) : IRequest<IEnumerable<SearchTermsDto>>;

    public class Handler : IRequestHandler<Query, IEnumerable<SearchTermsDto>>
    {
        private readonly IMediator _mediator;

        public Handler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IEnumerable<SearchTermsDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var today = DateTime.Today.Date;
            var lookbackDate = today.AddDays(-request.LookbackDays).Date;
            var search_term_query = @$"SELECT
                                search_term_view.search_term,
                                search_term_view.status,
                                ad_group.id,
                                ad_group.name,
                                campaign.id,
                                campaign.name,
                                metrics.impressions,
                                metrics.clicks,
                                metrics.conversions,
                                metrics.conversions_value,
                                metrics.cost_micros
                            FROM search_term_view
                            WHERE campaign.status != 'REMOVED'
                                AND ad_group.status != 'REMOVED'
                                AND campaign.advertising_channel_type = 'SEARCH'
                                AND ad_group.type = 'SEARCH_STANDARD'
                                AND segments.date > '{lookbackDate.ToString("yyyy-MM-dd")}'
                                AND segments.date <= '{today.ToString("yyyy-MM-dd")}'
                            ORDER BY metrics.clicks DESC
                            LIMIT 100";

            var dsa_search_term_query = $@"SELECT
                                dynamic_search_ads_search_term_view.search_term,
                                dynamic_search_ads_search_term_view.has_matching_keyword,
                                ad_group.id,
                                ad_group.name,
                                campaign.id,
                                campaign.name,
                                metrics.impressions,
                                metrics.clicks,
                                metrics.conversions,
                                metrics.conversions_value,
                                metrics.cost_micros
                            FROM dynamic_search_ads_search_term_view
                            WHERE campaign.status != 'REMOVED'
                                AND ad_group.status != 'REMOVED'
                                AND segments.date > '{lookbackDate.ToString("yyyy-MM-dd")}'
                                AND segments.date <= '{today.ToString("yyyy-MM-dd")}'
                            ORDER BY metrics.clicks DESC
                            LIMIT 100";

            var task1 = _mediator.Send(new GetSearchTermBaseQuery.Query(search_term_query, request.CustomerId));
            var task2 = _mediator.Send(new GetSearchTermBaseQuery.Query(dsa_search_term_query, request.CustomerId));

            var result1 = await task1;
            var result2 = await task2;

            var result = result1.Concat(result2).OrderByDescending(x => x.Clicks).ToList();
            return result;

        }
    }
}