using Microsoft.AspNetCore.Mvc;
using System.Net;
using MediatR;
using Veveve.Api.Domain.Commands.Clients;
using Microsoft.AspNetCore.Authorization;
using Veveve.Api.Domain.Queries.Clients;
using Veveve.Api.Infrastructure.Authorization;
using Veveve.Api.Infrastructure.Swagger;
using Veveve.Api.Infrastructure.ErrorHandling;

namespace Veveve.Api.Controllers.Clients;

/// <summary>
/// Clients API
/// </summary>
[Route("api/v1/[controller]")]
[ApiController]
public class ClientsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IJwtTokenHelper _jwtTokenHelper;

    public ClientsController(
        IMediator mediator,
        IJwtTokenHelper jwtTokenHelper)
    {
        _mediator = mediator;
        _jwtTokenHelper = jwtTokenHelper;
    }

    /// <summary>
    /// [Admin] Create a new Client.
    /// </summary>
    [Authorize(AuthPolicies.Admin)]
    [HttpPost(Name = nameof(CreateClient))]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [SwaggerErrorCodes(HttpStatusCode.Conflict, ErrorCodesEnum.CLIENT_NAME_ALREADY_EXISTS)]
    public async Task<ActionResult<ClientResponse>> CreateClient([FromBody] CreateClientRequest body)
    {
        var account = await _mediator.Send(new CreateClient.Command(body.Name));
        return Created("", new ClientResponse(account));
    }

    /// <summary>
    /// [Admin] Update an existing Client
    /// </summary>
    [Authorize(AuthPolicies.Admin)]
    [HttpPut("{id}", Name = nameof(UpdateClient))]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [SwaggerErrorCodes(HttpStatusCode.NotFound, ErrorCodesEnum.CLIENT_ID_DOESNT_EXIST)]
    [SwaggerErrorCodes(HttpStatusCode.Conflict, ErrorCodesEnum.CLIENT_NAME_ALREADY_EXISTS)]
    public async Task<ActionResult<ClientResponse>> UpdateClient(
        [FromRoute] int id,
        [FromBody] UpdateClientRequest body)
    {
        var account = await _mediator.Send(new UpdateClient.Command(id, body.Name));
        return Ok(new ClientResponse(account));
    }

    /// <summary>
    /// [Admin] Get Clients
    /// </summary>
    [Authorize(AuthPolicies.Admin)]
    [HttpGet(Name = nameof(GetClients))]
    [Produces("application/json")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<ClientResponse>>> GetClients()
    {
        var clients = await _mediator.Send(new GetClients.Query());
        return Ok(clients.Select(x => new ClientResponse(x)));
    }

    /// <summary>
    /// [Admin] Delete Client.
    /// </summary>
    [Authorize(AuthPolicies.Admin)]
    [HttpDelete("{id}", Name = nameof(DeleteClient))]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [SwaggerErrorCodes(HttpStatusCode.NotFound, ErrorCodesEnum.CLIENT_ID_DOESNT_EXIST)]
    public async Task<ActionResult> DeleteClient(
        [FromRoute] int id)
    {
        await _mediator.Send(new DeleteClient.Command(id));
        return NoContent();
    }
}