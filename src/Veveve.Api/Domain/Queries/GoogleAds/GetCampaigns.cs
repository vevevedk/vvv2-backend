using MediatR;
using Veveve.Api.Domain.Services;

namespace Veveve.Api.Domain.Commands.GoogleAds
{
    public static class GetCampaigns
    {
        public record Command() : IRequest;
        public class Handler : AsyncRequestHandler<Command>
        {
            private readonly IGoogleAdsClientFacade _googleAdsClientFacade;
            public Handler(IGoogleAdsClientFacade clientFacade)
            {
                _googleAdsClientFacade = clientFacade;
            }

            protected override async Task Handle(Command request, CancellationToken cancellationToken)
            {
                await _googleAdsClientFacade.InitialTest();
            }
        }
    }
}
