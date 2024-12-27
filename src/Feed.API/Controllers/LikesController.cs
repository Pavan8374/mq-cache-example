    using Feed.API.Manager.UserLikes;
    using Feed.API.Models.Likes;
    using Microsoft.AspNetCore.Mvc;

    namespace Feed.API.Controllers
    {
    [ApiController]
    [Route("/v{version:ApiVersion}/[controller]")]
    public class LikesController : ControllerBase
        {
            private readonly ILikeManager _likeManager;
            private readonly ILogger<LikesController> _logger;
            public LikesController(ILikeManager likeManager, ILogger<LikesController> logger)
            {
                _likeManager = likeManager;
                _logger = logger;
            }

            [HttpPost]
            public async Task<IActionResult> LikePost([FromBody] LikeRequest request)
            {
                try
                {
                    await _likeManager.HandleLikeAsync(request);
                    return Ok("User liked the post");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing like request");
                    return StatusCode(500, "An error occurred while processing your request");
                }
            }

            [HttpGet("{userId}/{postId}")]
            public async Task<IActionResult> IsLiked(Guid userId, Guid postId)
            {
                try
                {
                    var result = await _likeManager.IsLikedAsync(userId, postId);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking like status");
                    return StatusCode(500, "An error occurred while processing your request");
                }
            }
        }
    }
