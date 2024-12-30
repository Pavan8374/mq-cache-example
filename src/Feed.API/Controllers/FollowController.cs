using Feed.API.Models.Follows;
using Feed.Domain.Follows;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Feed.API.Controllers
{
    [ApiController]
    [Route("/v{version:ApiVersion}/[controller]")]
    public class FollowController : ControllerBase
    {
        private readonly IFolowService _followService; 
        public FollowController(IFolowService folowService)
        {
            _followService = folowService;
        }

        [Authorize]
        [HttpPost("{followingId}")]
        public async Task<IActionResult> Follow_UnFollow_Account(Guid followingId, FollowEnum followEnum)
        {
            try
            {
                Guid followerId = Guid.NewGuid(); //get from context or beartoken claims
                var followRecord = await _followService.GetFollowRecord(followerId, followingId);

                if (followEnum == FollowEnum.Follow)
                {
                    if(followRecord != null)
                    {
                        return StatusCode(500, "Already following the user account!");
                    }
                }

                if(followEnum == FollowEnum.UnFolllow)
                {
                    if(followRecord == null)
                    {
                        return StatusCode(500, "An error occurred while processing your request");
                    }
                    await _followService.DeleteAsync(followRecord);
                    return StatusCode(200, "Unfollowed successfully!");
                }
                var follow = new Follow()
                {
                    FollowerId = followerId,
                    FollowingId = followingId,
                };
                var newFollow = await _followService.AddAsync(follow);
                return StatusCode(200, newFollow);

            }
            catch(Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request");
            }

        }
    }
}
