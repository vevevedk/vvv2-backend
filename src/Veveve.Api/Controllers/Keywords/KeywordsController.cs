using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Veveve.Api.Authorization;
using Veveve.Api.Swagger;
using Microsoft.AspNetCore.Authorization;
using Veveve.Domain.Commands.Keywords;
using Veveve.Domain.Models.Jobs;
using Veveve.Api.Controllers.Jobs;

namespace Veveve.Api.Controllers.Keywords;

[Route("api/v1/[controller]")]
[ApiController]
public class KeywordsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IJwtTokenHelper _jwtTokenHelper;

    public KeywordsController(
        IMediator mediator,
        IJwtTokenHelper jwtTokenHelper)
    {
        _mediator = mediator;
        _jwtTokenHelper = jwtTokenHelper;
    }

    /// <summary>
    /// Create keywords for a given Google Ads Customer ID
    /// </summary>
    /// <remarks>
    /// A background job will be created to perform the keyword creation.
    /// </remarks>
    /// <returns>
    /// The ID of the background job.
    /// </returns>
    [HttpPost(Name = nameof(PostKeywords))]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.Created)] 
    [Authorize]
    public async Task<ActionResult<JobResponse>> PostKeywords([FromBody] CreateKeywordsRequest request)
    {
        var clientId = _jwtTokenHelper.GetClientId()!.Value;
        var keywords = request.Keywords.Select(x => new CreateKeywordsItem(x.KeywordText, x.AdGroupId));
        var job = await _mediator.Send(new CreateKeywords.Command(clientId, request.GoogleAdsCustomerId, request.Negative, keywords));
        return Created(string.Empty, new JobResponse(job));
    }
}
