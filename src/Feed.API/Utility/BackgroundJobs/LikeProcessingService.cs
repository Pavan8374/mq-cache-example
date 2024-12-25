
using Feed.API.Models.Likes;
using Feed.Domain.UserInteractions;
using Feed.MQ.MessageQueue;

namespace Feed.API.Utility.BackgroundJobs
{
    public class LikeProcessingService : BackgroundService
    {
        private readonly IMessageQueueService _messageQueueService;
        private readonly IServiceProvider _serviceProvider; // Use IServiceProvider
        private readonly ILogger<LikeProcessingService> _logger;
        private const string QUEUE_NAME = "likes_queue";

        public LikeProcessingService(
            IMessageQueueService messageQueueService,
            IServiceProvider serviceProvider, // Inject IServiceProvider
            ILogger<LikeProcessingService> logger)
        {
            _messageQueueService = messageQueueService;
            _serviceProvider = serviceProvider; // Store the provider
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _messageQueueService.ConsumeAsync<LikeRequest>(QUEUE_NAME, async request =>
            {
                using (var scope = _serviceProvider.CreateScope()) // Create a scope
                {
                    var userLikeService = scope.ServiceProvider.GetRequiredService<IUserLikeService>(); // Resolve IUserLikeService

                    var like = new UserLike
                    {
                        UserId = request.UserId,
                        ContentId = request.PostId,
                        CreatedAt = DateTime.UtcNow,
                    };

                    await userLikeService.AddAsync(like);
                }
            });

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }

}
