using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Veveve.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using Veveve.Domain.Queries.Jobs;

namespace Veveve.Api.Controllers.Jobs;

[Route("api/v1/[controller]")]
[ApiController]
public class JobsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IJwtTokenHelper _jwtTokenHelper;

    public JobsController(
        IMediator mediator,
        IJwtTokenHelper jwtTokenHelper)
    {
        _mediator = mediator;
        _jwtTokenHelper = jwtTokenHelper;
    }

    [HttpPost(Name = nameof(GetJobs))]
    [Produces("application/json")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [Authorize]
    public async Task<ActionResult<IEnumerable<JobResponse>>> GetJobs(
        [FromQuery, Required] JobFeatureNameEnum featureName)
    {
        var clientId = _jwtTokenHelper.GetClientId()!.Value;
        var jobs = await _mediator.Send(new GetJobs.Query(clientId, featureName));
        return Ok(jobs.Select(dto => new JobResponse(dto)));
    }
}
