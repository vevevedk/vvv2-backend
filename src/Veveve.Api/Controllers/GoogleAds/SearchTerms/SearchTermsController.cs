using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Veveve.Api.Infrastructure.Authorization;
using Veveve.Api.Domain.Commands.GoogleAds;
using Veveve.Api.Controllers.GoogleAds.SearchTerms;
using Microsoft.AspNetCore.Authorization;
using Veveve.Api.Infrastructure.Swagger;

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
    /// <param name="body"></param>
    /// <returns></returns>
    [HttpPost(Name = nameof(GetSearchTerms))]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [SwaggerErrorCodes(HttpStatusCode.InternalServerError)]
    //[Authorize]
    public async Task<ActionResult<GetSearchTermsResponse>> GetSearchTerms([FromBody] GetSearchTermRequest body)
    {
        try
        {
            var searchTerms = await _mediator.Send(new GetSearchTerms.Query(body.GoogleAdsCustomerId, body.LookbackDays));
            return Ok(searchTerms.Select(dto => new GetSearchTermsResponse(dto)));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
