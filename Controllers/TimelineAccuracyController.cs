using Microsoft.AspNetCore.Mvc;
using Microsoft.PowerPlatform.Dataverse.Client;
using Ultratechapis.Service;
using Ultratechapis.Data;
using System;

namespace Ultratechapis.Controllers
{
    [Route("api/timelineaccuracy")]
    [ApiController]
    public class TimelineAccuracyController : ControllerBase
    {
        [HttpGet("user")]
        public IActionResult GetTimelineAccuracyForUser([FromQuery] Guid userId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromServices] Connect connect)
        {
            try
            {
                var service = new TimelineAccuracyService(connect.Client);
                var result = service.CalculateTimelineAccuracy(userId, startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
