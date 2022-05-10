using Microsoft.AspNetCore.Mvc;
using System.Net;
using MediatR;
using Veveve.Api.Domain.Commands.Accounts;
using Microsoft.AspNetCore.Authorization;
using Veveve.Api.Domain.Queries.Accounts;
using Veveve.Api.Infrastructure.Authorization;
using Veveve.Api.Infrastructure.Swagger;
using Veveve.Api.Infrastructure.ErrorHandling;

namespace Veveve.Api.Controllers.Accounts;

/// <summary>
/// Accounts API
/// </summary>
[Route("api/v1/[controller]")]
[ApiController]
public class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IJwtTokenHelper _jwtTokenHelper;

    public AccountsController(
        IMediator mediator,
        IJwtTokenHelper jwtTokenHelper)
    {
        _mediator = mediator;
        _jwtTokenHelper = jwtTokenHelper;
    }

    /// <summary>
    /// [Admin] Create a new Account.
    /// </summary>
    [Authorize(AuthPolicies.Admin)]
    [HttpPost(Name = nameof(CreateAccount))]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [SwaggerErrorCodes(HttpStatusCode.Conflict, ErrorCodesEnum.ACCOUNT_GOOGLEADSID_ALREADY_EXIST)]
    public async Task<ActionResult<AccountResponse>> CreateAccount([FromBody] CreateAccountRequest body)
    {
        var account = await _mediator.Send(new CreateAccount.Command(body.ClientId, body.GoogleAdsAccountId, body.GoogleAdsAccountName));
        return Created("", new AccountResponse(account));
    }

    /// <summary>
    /// [Admin] Update an existing Account
    /// </summary>
    [Authorize(AuthPolicies.Admin)]
    [HttpPut("{id}", Name = nameof(UpdateAccount))]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [SwaggerErrorCodes(HttpStatusCode.NotFound, ErrorCodesEnum.ACCOUNT_ID_DOESNT_EXIST)]
    [SwaggerErrorCodes(HttpStatusCode.Conflict, ErrorCodesEnum.ACCOUNT_GOOGLEADSID_ALREADY_EXIST)]
    public async Task<ActionResult<AccountResponse>> UpdateAccount(
        [FromRoute] int id,
        [FromBody] UpdateAccountRequest body)
    {
        var account = await _mediator.Send(new UpdateAccount.Command(id, body.GoogleAdsAccountId, body.GoogleAdsAccountName));
        return Ok(new AccountResponse(account));
    }

    /// <summary>
    /// Get Accounts
    /// </summary>
    [Authorize]
    [HttpGet(Name = nameof(GetAccounts))]
    [Produces("application/json")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<AccountResponse>>> GetAccounts()
    {
        var jwtClientId = _jwtTokenHelper.GetClientId()!.Value;
        var accounts = await _mediator.Send(new GetAccounts.Query(jwtClientId));
        return Ok(accounts.Select(x => new AccountResponse(x)));
    }

    /// <summary>
    /// [Admin] Delete Account.
    /// </summary>
    [Authorize(AuthPolicies.Admin)]
    [HttpDelete("{id}", Name = nameof(DeleteAccount))]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [SwaggerErrorCodes(HttpStatusCode.NotFound, ErrorCodesEnum.ACCOUNT_ID_DOESNT_EXIST)]
    public async Task<ActionResult> DeleteAccount(
        [FromRoute] int id)
    {
        await _mediator.Send(new DeleteAccount.Command(id));
        return NoContent();
    }
}