using Feed.API.Utility.Analytics;
using Microsoft.AspNetCore.Mvc;

namespace Feed.API.Controllers
{
    [ApiController]
    [Route("/v{version:ApiVersion}/[controller]")]

    public class UserController : ControllerBase
    {
        private readonly UserTracker _userTracker;

        public UserController(UserTracker userTracker)
        {
            _userTracker = userTracker;
        }

        [HttpGet("concurrent-users")]
        public IActionResult GetConcurrentUsers()
        {
            return Ok(new { count = _userTracker.GetConcurrentUsers() });
        }


    }
}
