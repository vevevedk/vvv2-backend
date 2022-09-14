using Microsoft.AspNetCore.Mvc;
using System.Net;
using MediatR;
using Veveve.Domain.Commands.Clients;
using Microsoft.AspNetCore.Authorization;
using Veveve.Domain.Queries.Clients;
using Veveve.Api.Authorization;
using Veveve.Api.Swagger;
using Veveve.Domain.Exceptions;
using Veveve.Domain.Database.Entities;

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

    /// <summary>
    /// [Admin] Assume a specific client
    /// </summary>
    /// <remarks>
    /// Gain a token with a clientId claim set to the clientId of the client you want to assume.
    /// </remarks>
    [Authorize(AuthPolicies.Admin)]
    [HttpPut("{id}/assume", Name = nameof(AssumeClient))]
    [Produces("application/json")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [SwaggerErrorCodes(HttpStatusCode.NotFound, ErrorCodesEnum.CLIENT_ID_DOESNT_EXIST)]
    public async Task<ActionResult<ClientAssumeResponse>> AssumeClient([FromRoute] int id)
    {
        var userId = _jwtTokenHelper.GetUserId();
        if (!userId.HasValue)
            throw new Exception("UserId is null but shouldn't be, because we are authenticated at this point");


        (UserEntity user, ClientEntity targetClient) = await _mediator.Send(new AssumeClient.Command(userId.Value, id));
        var jwtToken = _jwtTokenHelper.GenerateJwtToken(user, targetClient);
        return Ok(new ClientAssumeResponse(jwtToken));
    }
}