using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Veveve.Api.Infrastructure.Authorization;
using Veveve.Api.Infrastructure.ErrorHandling;
using Veveve.Api.Infrastructure.Swagger;
using Veveve.Api.Domain.Commands.GoogleAds;

namespace Veveve.Api.Controllers.GoogleAds;

[Route("api/v1/[controller]")]
[ApiController]
public class CampaignsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IJwtTokenHelper _jwtTokenHelper;

    public CampaignsController(
        IMediator mediator,
        IJwtTokenHelper jwtTokenHelper)
    {
        _mediator = mediator;
        _jwtTokenHelper = jwtTokenHelper;
    }

    [HttpGet(Name = nameof(GetCampaigns))]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    //[SwaggerErrorCodes()]
    public async Task<IActionResult> GetCampaigns()
    {
        try
        {
            await _mediator.Send(new GetCampaigns.Command());
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
