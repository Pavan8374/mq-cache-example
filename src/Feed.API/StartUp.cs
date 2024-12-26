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
        private IConfiguration Configuration { get; }

        public StartUp(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RabbitMQSettings>(Configuration.GetSection("RabbitMQSettings"));

            // Register Redis Cache
             services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration.GetValue<string>("Redis:ConnectionString") ?? "localhost:6379";
            });

            // Register RabbitMQ services
            services.AddSingleton<IConnectionFactory>(sp =>
            {
                var rabbitConfig = Configuration.GetSection("RabbitMQSettings").Get<RabbitMQSettings>();

                return new ConnectionFactory
                {
                    HostName = rabbitConfig?.Host ?? "localhost",
                    UserName = rabbitConfig?.UserName ?? "guest",
                    Password = rabbitConfig?.Password ?? "guest",
                    VirtualHost = rabbitConfig?.VirtualHost ?? "/",
                    Port = rabbitConfig?.Port ?? 5672,
                    RequestedHeartbeat = TimeSpan.FromSeconds(60),
                    AutomaticRecoveryEnabled = true
                };
            });

            // Register message queue service
            services.AddSingleton<IMessageQueueService, RabbitMQService>();

            // Register cache service
            services.AddScoped<ICacheService, RedisCacheService>();

            // Register LikeManager
            services.AddScoped<ILikeManager, LikeManager>();

            // Add background service for processing likes
            services.AddHostedService<LikeProcessingService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }
        }
    }
}