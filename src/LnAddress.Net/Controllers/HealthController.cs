using Microsoft.AspNetCore.Mvc;

namespace LnAddress.Net.Controllers;

using Interfaces;

[ApiController]
[Route("[controller]")]
public class HealthController(ILightningService lightningService): ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> Get()
    {
        try
        {
            var isConnected = await lightningService.CheckConnection();
            return isConnected
                ? Ok(new { ok = true })
                : StatusCode(StatusCodes.Status503ServiceUnavailable, new { ok = false });
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { ok = false });
        }
    }
}