////using Microsoft.AspNetCore.Mvc;
////using Microsoft.Extensions.Caching.Memory;
////using Microsoft.PowerPlatform.Dataverse.Client;
////using Ultratechapis.Data;
////using Ultratechapis.Service;

////namespace Ultratechapis.Controllers
////{
////    [Route("api/sob")]
////    [ApiController]
////    public class SobController : ControllerBase
////    {
////        private readonly IMemoryCache _cache;

////        public SobController(IMemoryCache cache)
////        {
////            _cache = cache;
////        }

////        [HttpGet("loggedin")]
////        public IActionResult GetSOBForLoggedUser([FromServices] Connect connect)
////        {
////            try
////            {
////                var sobService = new SobService(connect.Client, _cache);
////                var result = sobService.CalculateSOBForLoggedInUser();
////                return Ok(result);
////            }
////            catch (Exception ex)
////            {
////                return StatusCode(500, new { error = ex.Message });
////            }
////        }

////        [HttpGet("byuserid")]
////        public IActionResult GetSOBByUserId([FromQuery] Guid userId, [FromServices] Connect connect)
////        {
////            try
////            {
////                var sobService = new SobService(connect.Client, _cache);
////                var result = sobService.CalculateSOBByUserId(userId);
////                return Ok(result);
////            }
////            catch (Exception ex)
////            {
////                return StatusCode(500, new { error = ex.Message });
////            }
////        }
////    }
////}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.PowerPlatform.Dataverse.Client;
using Ultratechapis.Data;
using Ultratechapis.Service;

namespace Ultratechapis.Controllers
{
    [Route("api/sob")]
    [ApiController]
    public class SobController : ControllerBase
    {
        private readonly IMemoryCache _cache;

        public SobController(IMemoryCache cache)
        {
            _cache = cache;
        }


        // API to calculate SOB Growth for a specific user (by UserId)
        [HttpGet("byuserid")]
        public IActionResult GetSOBByUserId([FromQuery] Guid userId, [FromServices] Connect connect, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var sobService = new SobService(connect.Client, _cache);
                var result = sobService.CalculateSOBGrowth(userId, startDate, endDate); // Passing userId from query parameters
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}

//using Microsoft.AspNetCore.Mvc;
//using Microsoft.PowerPlatform.Dataverse.Client;
//using Ultratechapis.Service;
//using System;

//namespace Ultratechapis.Controllers
//{
//    [Route("api/sobgrowth")]
//    [ApiController]
//    public class SobController : ControllerBase
//    {
//        private readonly SobService _sobService;

//        public SobController(SobService sobGrowthService)
//        {
//            _sobService = sobGrowthService;
//        }

//        // Endpoint to get SOB Growth for a specific user
//        [HttpGet("calculate")]
//        public IActionResult GetSOBGrowth([FromQuery] Guid userId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
//        {
//            try
//            {
//                var result = _sobService.CalculateSOBGrowth(userId, startDate, endDate);
//                return Ok(result);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { error = ex.Message });
//            }
//        }
//    }
//}

