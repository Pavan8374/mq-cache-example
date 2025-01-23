using Feed.API.Utility.Analytics;
using Feed.Domain.Users;
using Microsoft.AspNetCore.Mvc;

namespace Feed.API.Controllers
{
    [ApiController]
    [Route("/v{version:ApiVersion}/[controller]")]

    public class UserController : ControllerBase
    {
        private readonly UserTracker _userTracker;
        private readonly IUserService _userService;

        public UserController(UserTracker userTracker, IUserService userService)
        {
            _userTracker = userTracker;
            _userService = userService;
        }

        [HttpGet("concurrent-users")]
        public IActionResult GetConcurrentUsers()
        {
            return Ok(new { count = _userTracker.GetConcurrentUsers() });
        }

        [HttpPost]
        [ProducesResponseType(typeof(List<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Seed()
        {
            try
            {
                int users = 1000;
                int f = 3;
                long lat = (long)21.17801000;
                long longi = (long)72.81189000;

                await _userService.SeedDummyData(users, f, lat, longi);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server");
            }
        }
    }
}
