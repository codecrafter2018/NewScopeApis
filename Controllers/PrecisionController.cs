using Microsoft.AspNetCore.Mvc;
using Microsoft.PowerPlatform.Dataverse.Client;
using Ultratechapis.Service;
using Ultratechapis.Data; 

namespace Ultratechapis.Controllers
{
    [Route("api/precision")]
    [ApiController]
    public class PrecisionController : ControllerBase
    {
        [HttpGet("user")]
        public IActionResult GetPrecisionForUser([FromQuery] Guid userId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromServices] Connect connect)
        {
            try
            {
                var service = new PrecisionService(connect.Client);
                var result = service.CalculatePrecision(userId,startDate,endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
  