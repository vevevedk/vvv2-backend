using Microsoft.AspNetCore.Mvc;
using System.Net;
using MediatR;
using Veveve.Api.Domain.Commands.Users;
using Microsoft.AspNetCore.Authorization;
using Veveve.Api.Domain.Queries.Users;
using Veveve.Api.Infrastructure.Authorization;
using Veveve.Api.Infrastructure.Swagger;
using Veveve.Api.Infrastructure.ErrorHandling;
using Veveve.Api.Domain.Exceptions;

namespace Veveve.Api.Controllers.Users;

/// <summary>
/// Users API
/// </summary>
[Route("api/v1/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IJwtTokenHelper _jwtTokenHelper;

    public UsersController(
        IMediator mediator,
        IJwtTokenHelper jwtTokenHelper)
    {
        _mediator = mediator;
        _jwtTokenHelper = jwtTokenHelper;
    }

    /// <summary>
    /// [Admin] Create a new User
    /// </summary>
    [Authorize(AuthPolicies.Admin)]
    [HttpPost(Name = nameof(CreateUser))]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [SwaggerErrorCodes(HttpStatusCode.Conflict, ErrorCodesEnum.USER_EMAIL_ALREADY_EXIST)]
    public async Task<ActionResult<UserResponse>> CreateUser([FromBody] CreateUserRequest body)
    {
        var jwtClientId = _jwtTokenHelper.GetClientId()!.Value;
        var User = await _mediator.Send(new CreateUser.Command(jwtClientId, body.FullName, body.Email, body.IsAdmin));
        return Created("", new UserResponse(User));
    }

    /// <summary>
    /// Update an existing User
    /// </summary>
    /// <remarks>
    /// This endpoint is only accessible to the user who is updating their own account.
    /// Admins can update any user.
    /// </remarks>
    [Authorize]
    [HttpPut("{id}", Name = nameof(UpdateUser))]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [SwaggerErrorCodes(HttpStatusCode.NotFound, ErrorCodesEnum.USER_ID_DOESNT_EXIST)]
    [SwaggerErrorCodes(HttpStatusCode.Conflict, ErrorCodesEnum.USER_EMAIL_ALREADY_EXIST)]
    public async Task<ActionResult<UserResponse>> UpdateUser(
        [FromRoute] int id,
        [FromBody] UpdateUserRequest body)
    {
        var jwtClientId = _jwtTokenHelper.GetClientId()!.Value;
        if (!_jwtTokenHelper.HasAdminClaim() && body.IsAdmin.HasValue)
            return StatusCode((int)HttpStatusCode.Forbidden, $"You must have admin rights to update the following properties: {nameof(body.IsAdmin)}");

        if (_jwtTokenHelper.GetUserId() != id && !_jwtTokenHelper.HasAdminClaim())
            return StatusCode((int)HttpStatusCode.Forbidden, $"You can only update your own account, unless you're an admin");

        var user = await _mediator.Send(new UpdateUser.Command(id, jwtClientId, body.FullName, body.Email, body.IsAdmin));
        return Ok(new UserResponse(user));
    }

    /// <summary>
    /// Get Users for a Client
    /// </summary>
    /// <remarks>
    /// Will use the client claim from the jwt token.
    /// </remarks>
    [Authorize]
    [HttpGet(Name = nameof(GetUsers))]
    [Produces("application/json")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsers(
    )
    {
        var jwtClientId = _jwtTokenHelper.GetClientId()!.Value;
        var users = await _mediator.Send(new GetUsers.Query(jwtClientId));
        return Ok(users.Select(x => new UserResponse(x)));
    }

    /// <summary>
    /// Login to an existing User
    /// </summary>
    /// <remarks>Returns a jwt token to be used with bearer authentication</remarks>
    [HttpPost("login", Name = nameof(LoginUser))]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [SwaggerErrorCodes(HttpStatusCode.UnprocessableEntity, ErrorCodesEnum.USER_LOGIN_EMAIL_OR_PASSWORD_INVALID)]
    public async Task<ActionResult<LoginResponse>> LoginUser([FromBody] LoginUserRequest body)
    {
        var loginResult = await _mediator.Send(new LoginUser.Command(body.Email, body.Password));
        return Ok(new LoginResponse(loginResult.JwtToken, loginResult.User));
    }

    /// <summary>
    /// Reset the password of an existing User.
    /// </summary>
    /// <remarks>If the email address exists, an email will be sent to the owner with a link to generate a new password.</remarks>
    [HttpPost("resetPassword", Name = nameof(ResetUserPassword))]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> ResetUserPassword([FromBody] ResetUserPasswordRequest body)
    {
        try
        {
            await _mediator.Send(new ResetUserPassword.Command(body.Email));
        }
        catch (NotFoundException) { }

        return NoContent();
    }

    /// <summary>
    /// Update the password of an User using the token from the reset password email.
    /// </summary>
    [HttpPut("updatePassword", Name = nameof(UpdateUserPassword))]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [SwaggerErrorCodes(HttpStatusCode.UnprocessableEntity, ErrorCodesEnum.USER_RESETPASSWORD_TOKEN_INVALID)]
    public async Task<IActionResult> UpdateUserPassword([FromBody] UpdateUserPasswordRequest body)
    {
        await _mediator.Send(new UpdateUserPassword.Command(body.ResetPasswordToken, body.Password));
        return NoContent();
    }

    /// <summary>
    /// Delete User.
    /// </summary>
    [Authorize(AuthPolicies.Admin)]
    [HttpDelete("{id}", Name = nameof(DeleteUser))]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [SwaggerErrorCodes(HttpStatusCode.NotFound, ErrorCodesEnum.USER_ID_DOESNT_EXIST)]
    public async Task<ActionResult> DeleteUser(
        [FromRoute] int id)
    {
        await _mediator.Send(new DeleteUser.Command(id));
        return NoContent();
    }
}