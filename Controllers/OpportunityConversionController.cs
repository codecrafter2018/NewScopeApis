using Microsoft.AspNetCore.Mvc;
using Microsoft.PowerPlatform.Dataverse.Client;
using Ultratechapis.Service;
using Ultratechapis.Data; 

namespace Ultratechapis.Controllers
{
    [Route("api/opportunityconversion")]
    [ApiController]
    public class OpportunityConversionController : ControllerBase
    {
        [HttpGet("user")]
        public IActionResult GetOpportunityConversionForUser([FromQuery] Guid userId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromServices] Connect connect)
        {
            try
            {
                var service = new OpportunityConversionService(connect.Client);
                var result = service.CalculateOpportunityConversion(userId,startDate , endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
