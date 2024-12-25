using Feed.API.Manager.UserLikes;
using Feed.API.Utility.BackgroundJobs;
using Feed.Cache.Cache;
using Feed.MQ.MessageQueue;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Feed.API
{
    public class StartUp
    {
        
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMQSettings>(configuration.GetSection("RabbitMQSettings"));
            // Register Redis Cache
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost:6379";
            });

            // Register RabbitMQ services
            services.AddSingleton<IConnectionFactory>(sp =>
            {
                return new ConnectionFactory
                {
                    HostName = "localhost",
                    // Add any additional RabbitMQ configuration here
                    RequestedHeartbeat = TimeSpan.FromSeconds(60),
                    AutomaticRecoveryEnabled = true
                };
            });

            // Register IConnection as singleton
            //services.AddSingleton<IConnection>(sp =>
            //{
            //    var factory = sp.GetRequiredService<IConnectionFactory>();
            //    return (IConnection)factory.CreateConnectionAsync();
            //});

            // Register IChannel (IModel in RabbitMQ.Client) as scoped
            //services.AddScoped<IChannel>(sp =>
            //{
            //    var connection = sp.GetRequiredService<IConnection>();
            //    return (IChannel)connection.CreateChannelAsync();
            //});

            // Register message queue service
            services.AddSingleton<IMessageQueueService, RabbitMQService>();

            // Register cache service
            services.AddScoped<ICacheService, RedisCacheService>();

            // Register LikeManager
            services.AddScoped<ILikeManager, LikeManager>();

            // Add background service for processing likes
            services.AddHostedService<LikeProcessingService>();
        }
    }
}
