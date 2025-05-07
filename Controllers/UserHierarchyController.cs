using Microsoft.AspNetCore.Mvc;
using Ultratechapis.Service;
using Ultratechapis.Data;
using Ultratechapis.Data;
using Ultratechapis.Service;

namespace Ultratechapis.Controllers
{
    [Route("api/userhierarchy")]
    [ApiController]
    public class UserHierarchyController : ControllerBase
    {
        [HttpGet("managerteam")]
        public IActionResult GetUsersUnderManager(
       [FromQuery] Guid? managerId,
       [FromQuery] string? managerName,
       [FromServices] Connect connect)
        {
            try
            {
                var service = new UserHierarchyService(connect.Client);
                var team = service.GetTeamByManager(managerName, managerId);

                return Ok(team);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("all-managers")]
        public IActionResult GetAllManagers([FromServices] Connect connect)
        {
            try
            {
                var service = new UserHierarchyService(connect.Client);
                var result = service.GetAllManagers();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("users-with-managers")]
        public IActionResult GetAllUsersWithManagers([FromServices] Connect connect)
        {
            try
            {
                var service = new UserHierarchyService(connect.Client);
                var users = service.GetAllUsersWithManagers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }



    }
}

