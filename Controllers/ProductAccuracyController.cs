using Microsoft.AspNetCore.Mvc;
using Microsoft.PowerPlatform.Dataverse.Client;
using Ultratechapis.Service;
using Ultratechapis.Data;

namespace Ultratechapis.Controllers
{
    [Route("api/productaccuracy")]
    [ApiController]
    public class ProductAccuracyController : ControllerBase
    {
        [HttpGet("calculate")]
        public IActionResult GetProductAccuracy([FromQuery] Guid userId, [FromQuery] Guid projectId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromServices] Connect connect)
        {
            try
            {
                var productAccuracyService = new ProductAccuracyService(connect.Client);
                var result = productAccuracyService.CalculateProductAccuracy(userId, projectId, startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
