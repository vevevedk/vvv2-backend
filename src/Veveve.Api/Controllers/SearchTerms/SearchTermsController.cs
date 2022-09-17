using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Veveve.Api.Authorization;
using Veveve.Domain.Queries.GoogleAds.SearchTerms;
using Veveve.Api.Swagger;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace Veveve.Api.Controllers.SearchTerms;

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
    /// <returns></returns>
    [HttpGet(Name = nameof(GetSearchTerms))]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [SwaggerErrorCodes(HttpStatusCode.InternalServerError)]
    [Authorize]
    public async Task<ActionResult<IEnumerable<SearchTermResponse>>> GetSearchTerms(
        [FromQuery, Required] string googleAdsCustomerId,
        [FromQuery, Required] int lookbackDays)
    {
        var searchTerms = await _mediator.Send(new GetSearchTerms.Query(googleAdsCustomerId, lookbackDays));
        return Ok(searchTerms.Select(dto => new SearchTermResponse(dto)));
    }
}
