using Microsoft.AspNetCore.Mvc;

namespace LnAddress.Net.Controllers;

using Models.Responses;

[ApiController]
[Route("[controller]")]
public class LnUrlPController : ControllerBase
{
    [HttpGet("{username}")]
    public ActionResult<LnAddressWellKnownResponse> Get(string username)
    {
        try
        {
            // Get the host called
            var domain = Request.Host.Value;
            // Get the scheme (http or https)
            var scheme = Request.Scheme;

            return new LnAddressWellKnownResponse($"{scheme}://{domain}/lnurl/callback/{username}", 1_000, 100_000_000)
                .WithMetadata($"""[["text/plain","Paying {username}"]]""")
                .WithCommentAllowed(256);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }
}