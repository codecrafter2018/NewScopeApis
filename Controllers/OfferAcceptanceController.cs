using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.PowerPlatform.Dataverse.Client;
using Ultratechapis.Data;
using Ultratechapis.Service;

namespace Ultratechapis.Controllers
{
    [Route("api/offeracceptancerate")]
    [ApiController]
    public class OfferAcceptanceController : ControllerBase
    {
        private readonly OfferService _offerService;
        private IMemoryCache memoryCache;

        public OfferAcceptanceController(OfferService offerService)
        {
            _offerService = offerService;
        }

        [HttpGet("alluserswithOCR")]
        public IActionResult GetAllUserByStatus()
        {
            var offerAcceptanceRate = _offerService.GetOfferAcceptanceRate();
            if (offerAcceptanceRate == null || offerAcceptanceRate.Count == 0)
            {
                return NotFound("No data found.");
            }
            return Ok(offerAcceptanceRate);
        }

        //[HttpGet("getsigneduserOCR")]
        //public IActionResult GetLoggedUserStatus()
        //{
        //    try
        //    {
        //        var userAcceptanceRate = _offerService.GetLoggedInUserAcceptanceRate();
        //        return Ok(userAcceptanceRate);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }
        //}

        [HttpGet("getuserOCRbyid")]
        public IActionResult GetUserStatusById([FromQuery] Guid userId)
        {
            try
            {
                var result = _offerService.GetUserAcceptanceRateById(userId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("connection-time")]
        public IActionResult GetConnectionTime([FromServices] Connect connect)
        {
            return Ok(new { connectTimeInMs = connect.ConnectionDuration.TotalMilliseconds });
        }

 
        [HttpGet("filtered-users")]
        public IActionResult GetFilteredUsers([FromServices] Connect connect)
        {
            try
            {
                var userService = new UserFilterService(connect.Client);
                var users = userService.GetFilteredUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

    }

    [Route("/")]
    [ApiController]
    public class RootController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetInfo()
        {
            var info = new
            {
                message = "Welcome to the Offer Acceptance Rate API.",
                availableEndpoints = new[]
                {
                    new { path = "/api/offeracceptancerate/alluserswithOCR", description = "Get acceptance rate of all users with OCR" },
                    //new { path = "/api/offeracceptancerate/getsigneduserOCR", description = "Get acceptance rate for the signed-in user with OCR" },
                    new { path = "/api/offeracceptancerate/getuserOCRbyid?userId=", description = "Get acceptance rate for a specific user by ID" },
                    new { path = "/api/offeracceptancerate/connection-time", description = "View Dataverse connection time in ms" },
                    new { path = "/api/sob/loggedin", description = "Get SOB data for the currently logged-in user" },
                    new { path = "/api/sob/byuserid?userId=", description = "Get SOB data for a specific user by ID" },
                }
            };
            return Ok(info);
        }
    }


} 
