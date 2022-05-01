using Microsoft.AspNetCore.Mvc;
using System.Net;
using MediatR;
using Veveve.Api.Domain.Commands.Emails;

namespace Veveve.Api.Controllers.SendGrid;

/// <summary>
/// SendGrid Webhooks
/// </summary>
[Route("api/v1/[controller]")]
[ApiController]
public class SendGridController : ControllerBase
{
    private readonly IMediator _mediator;

    public SendGridController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Receive Sendgrid webhook
    /// </summary>
    [HttpPost]
    [SendGridAuthorization]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult> Post([FromBody] IEnumerable<SendGridWebhookEventRequest> body)
    {
        // sometimes events come in bulk, sometimes one by one. Always look for the latest
        var latestEvent = body.MaxBy(x => x.Timestamp)!;

        var eventEnum = latestEvent.EventAsEnum;
        if (eventEnum == null)
            return Ok(); // not something we are tracking.

        var account = await _mediator.Send(new UpdateEmailLog.Command(latestEvent.Reference, eventEnum.Value));
        return Ok();
    }
}