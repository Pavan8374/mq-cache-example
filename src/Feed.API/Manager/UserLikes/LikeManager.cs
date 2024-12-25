using Feed.API.Models.Likes;
using Feed.Cache.Cache;
using Feed.MQ.MessageQueue;

namespace Feed.API.Manager.UserLikes
{

    public class LikeManager : ILikeManager
    {
        private readonly ICacheService _cacheService;
        private readonly IMessageQueueService _messageQueueService;
        private readonly ILogger<LikeManager> _logger;
        private const string QUEUE_NAME = "likes_queue";
        private const string CACHE_KEY_FORMAT = "like_{0}_{1}";

        public LikeManager(
        ICacheService cacheService,
        IMessageQueueService messageQueueService,
        ILogger<LikeManager> logger)
        {
            _cacheService = cacheService;
            _messageQueueService = messageQueueService;
            _logger = logger;
        }

        public async Task HandleLikeAsync(LikeRequest request)
        {
            try
            {
                // Store in cache
                var cacheKey = GetCacheKey(request.UserId, request.PostId);
                await _cacheService.SetAsync(cacheKey, true, TimeSpan.FromDays(7));

                // Publish to queue
                await _messageQueueService.PublishAsync(QUEUE_NAME, request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling like request for user {UserId} and post {PostId}",
                    request.UserId, request.PostId);
                throw;
            }
        }

        public async Task<bool> IsLikedAsync(Guid userId, Guid postId)
        {
            try
            {
                var cacheKey = GetCacheKey(userId, postId);
                return await _cacheService.GetAsync<bool>(cacheKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking like status for user {UserId} and post {PostId}",
                    userId, postId);
                throw;
            }
        }

        private static string GetCacheKey(Guid userId, Guid postId) =>
            string.Format(CACHE_KEY_FORMAT, userId, postId);
    }
}
