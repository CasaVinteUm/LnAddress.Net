using LnAddress.Net.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LnAddress.Net.Controllers;

using Models.Responses;

[ApiController]
[Route("[controller]")]
public class LnUrlController(ILightningService lightningService) : ControllerBase
{
    [HttpGet("callback/{username}")]
    public async Task<ActionResult<LnUrlCallbackResponse>> Get(string username, [FromQuery] long amount, [FromQuery] string? comment)
    {
        try
        {
            if (amount is < 1_000 or > 100_000_000)
            {
                return BadRequest(new ErrorResponse("Amount is outside bounds."));
            }

            if (comment?.Length > 256)
            {
                comment = comment[..256];
            }

            var pr = await lightningService.FetchInvoiceAsync(amount, username, comment);
            return new LnUrlCallbackResponse(pr);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(e.Message));
        }
    }
}