using Microsoft.AspNetCore.Mvc;
using System.Net;
using MediatR;
using Veveve.Api.Domain.Commands.Accounts;
using Microsoft.AspNetCore.Authorization;
using Veveve.Api.Domain.Queries.Accounts;
using Veveve.Api.Infrastructure.Authorization;
using Veveve.Api.Infrastructure.Swagger;
using Veveve.Api.Infrastructure.ErrorHandling;
using Veveve.Api.Domain.Exceptions;

namespace Veveve.Api.Controllers.Accounts;

/// <summary>
/// Accounts API
/// </summary>
[Route("api/v1/[controller]")]
[ApiController]
public class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new account
    /// </summary>
    [Authorize(AuthPolicies.Admin)]
    [HttpPost(Name = nameof(CreateAccount))]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [SwaggerErrorCodes(HttpStatusCode.Conflict, ErrorCodesEnum.ACCOUNT_EMAIL_ALREADY_EXIST)]
    public async Task<ActionResult<AccountResponse>> CreateAccount([FromBody] CreateAccountRequest body)
    {
        var account = await _mediator.Send(new CreateAccount.Command(body.FullName, body.Email, body.IsAdmin));
        return Created("", new AccountResponse(account));
    }

    /// <summary>
    /// Update an existing account
    /// </summary>
    [Authorize]
    [HttpPut("{id}", Name = nameof(UpdateAccount))]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [SwaggerErrorCodes(HttpStatusCode.NotFound, ErrorCodesEnum.ACCOUNT_ID_DOESNT_EXIST)]
    [SwaggerErrorCodes(HttpStatusCode.Conflict, ErrorCodesEnum.ACCOUNT_EMAIL_ALREADY_EXIST)]
    public async Task<ActionResult<AccountResponse>> UpdateAccount(
        [FromRoute] int id,
        [FromBody] UpdateAccountRequest body)
    {
        var account = await _mediator.Send(new UpdateAccount.Command(id, body.FullName, body.Email));
        return Ok(new AccountResponse(account));
    }


    /// <summary>
    /// Update an account to have admin permissions
    /// </summary>
    [Authorize(AuthPolicies.Admin)]
    [HttpPut("{id}/updateIsAdmin", Name = nameof(UpdateAccountIsAdmin))]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [SwaggerErrorCodes(HttpStatusCode.NotFound, ErrorCodesEnum.ACCOUNT_ID_DOESNT_EXIST)]
    public async Task<IActionResult> UpdateAccountIsAdmin(
        [FromRoute] int id,
        [FromBody] UpdateAccountIsAdminRequest body)
    {
        await _mediator.Send(new UpdateAccountIsAdmin.Command(id, body.IsAdmin));
        return NoContent();
    }

    /// <summary>
    /// Get accounts
    /// </summary>
    [Authorize(AuthPolicies.Admin)]
    [HttpGet(Name = nameof(GetAccounts))]
    [Produces("application/json")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<AccountResponse>>> GetAccounts()
    {
        var accounts = await _mediator.Send(new GetAccounts.Query());
        return Ok(accounts.Select(x => new AccountResponse(x)));
    }

    /// <summary>
    /// Login to an existing account
    /// </summary>
    /// <remarks>Returns a jwt token to be used with bearer authentication</remarks>
    [HttpPost("login", Name = nameof(LoginAccount))]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [SwaggerErrorCodes(HttpStatusCode.UnprocessableEntity, ErrorCodesEnum.ACCOUNT_LOGIN_EMAIL_OR_PASSWORD_INVALID)]
    public async Task<ActionResult<LoginResponse>> LoginAccount([FromBody] LoginAccountRequest body)
    {
        var loginResult = await _mediator.Send(new LoginAccount.Command(body.Email, body.Password));
        return Ok(new LoginResponse(loginResult.JwtToken, loginResult.Account));
    }

    /// <summary>
    /// Reset the password of an existing account.
    /// </summary>
    /// <remarks>If the email address exists, an email will be sent to the owner with a link to generate a new password.</remarks>
    [HttpPost("resetPassword", Name = nameof(ResetAccountPassword))]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> ResetAccountPassword([FromBody] ResetAccountPasswordRequest body)
    {
        try
        {
            await _mediator.Send(new ResetAccountPassword.Command(body.Email));
        }
        catch (NotFoundException) { }

        return NoContent();
    }

    /// <summary>
    /// Update the password of an account using the token from the reset password email.
    /// </summary>
    [HttpPut("updatePassword", Name = nameof(UpdateAccountPassword))]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [SwaggerErrorCodes(HttpStatusCode.UnprocessableEntity, ErrorCodesEnum.ACCOUNT_RESETPASSWORD_TOKEN_INVALID)]
    public async Task<IActionResult> UpdateAccountPassword([FromBody] UpdateAccountPasswordRequest body)
    {
        await _mediator.Send(new UpdateAccountPassword.Command(body.ResetPasswordToken, body.Password));
        return NoContent();
    }

    /// <summary>
    /// Delete account.
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