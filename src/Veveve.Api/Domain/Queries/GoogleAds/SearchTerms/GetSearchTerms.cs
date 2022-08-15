using MediatR;
using Veveve.Api.Domain.Services;

namespace Veveve.Api.Domain.Commands.GoogleAds
{
    public static class GetSearchTerms
    {
        public record Query(string CustomerId, int LookbackDays) : IRequest<IEnumerable<SearchTermsDto>>;
        
        public class Handler : IRequestHandler<Query, IEnumerable<SearchTermsDto>>
        {
            private readonly IGoogleAdsClientFacade _googleAdsClientFacade;
            public Handler(IGoogleAdsClientFacade clientFacade)
            {
                _googleAdsClientFacade = clientFacade;
            }

            public async Task<IEnumerable<SearchTermsDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var res1 = await _googleAdsClientFacade.GetSearchTermsDynamicSearchAds(request.CustomerId, request.LookbackDays);
                //var res2 = await _googleAdsClientFacade.GetSearchTerms(request.CustomerId, request.LookbackDays); // maybe I want the dto instead?
                


                // TODO join the results together
                return res1; // TODO fix
            }
        }
    }
}
