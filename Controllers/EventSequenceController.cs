using Microsoft.AspNetCore.Mvc;
using Microsoft.PowerPlatform.Dataverse.Client;
using Ultratechapis.Service;
using Ultratechapis.Data;

namespace Ultratechapis.Controllers
{
    [Route("api/eventsequence")]
    [ApiController]
    public class EventSequenceController : ControllerBase
    {
        // API to Validate Event Sequence for a given UserId, StartDate, and EndDate
        [HttpGet("validate")]
        public IActionResult ValidateEventSequence([FromQuery] Guid userId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromServices] Connect connect)
        {
            try
            {
                var service = new EventSequenceValidationService(connect.Client);
                var result = service.ValidateEventSequence(userId, startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
