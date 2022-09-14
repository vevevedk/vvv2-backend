using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Veveve.Domain.Authorization;
using Veveve.Domain.Commands.GoogleAds;
using Veveve.Api.Controllers.GoogleAds.SearchTerms;
using Veveve.Domain.Swagger;
using Microsoft.AspNetCore.Authorization;

namespace Veveve.Api.Controllers.GoogleAds;

[Route("api/v1/[controller]")]
[ApiController]
public class SearchTermsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IJwtTokenHelper _jwtTokenHelper;

    public SearchTermsController(
        IMediator mediator,
        IJwtTokenHelper jwtTokenHelper)
    {
        _mediator = mediator;
        _jwtTokenHelper = jwtTokenHelper;
    }

    /// <summary>
    /// Gets all search terms for a given Google Ads Customer ID and with a number of days of which to evaluate
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet(Name = nameof(GetSearchTerms))]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [SwaggerErrorCodes(HttpStatusCode.InternalServerError)]
    [Authorize]
    public async Task<ActionResult<IEnumerable<SearchTermResponse>>> GetSearchTerms([FromQuery] GetSearchTermRequest request)
    {
        var searchTerms = await _mediator.Send(new GetSearchTermsDynamicSearchAds.Query(request.GoogleAdsCustomerId, request.LookbackDays));
        return Ok(searchTerms.Select(dto => new SearchTermResponse(dto)));
    }
}
