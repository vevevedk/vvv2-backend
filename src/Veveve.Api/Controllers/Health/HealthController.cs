using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Veveve.Api.Controllers.Healths;

[Route("[controller]")]
[ApiController]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Health check endpoint.
    /// </summary>
    [HttpGet(Name = nameof(Get))]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult Get()
    {
        return Ok();
    }
}