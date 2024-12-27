using Asp.Versioning;
using Feed.API.Manager.UserLikes;
using Feed.API.Utility.BackgroundJobs;
using Feed.Cache.Cache;
using Feed.Domain.Contents;
using Feed.Domain.UserInteractions;
using Feed.Domain.Users;
using Feed.EF;
using Feed.EF.Repositories;
using Feed.MQ.MessageQueue;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddOpenApi();
            services.AddCors();

            ConfigureSqlServerContext(services);

            services.AddHttpClient();

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

            AddServices(services);
            AddVersioning(services);
        }
        private void AddServices(IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserService, UserService>();

            services.AddTransient<IContentRepository, ContentRepository>();
            services.AddTransient<IContentService, ContentService>();

            services.AddTransient<IUserLikeRepository, UserLikeRepository>();
            services.AddTransient<IUserLikeService, UserLikeService>();
        }
        //protected virtual void AddAuthorization(IServiceCollection services)
        //{

        //}
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseCors(options =>
            {
                options
                    .WithOrigins(Configuration.GetValue<string>("AllowedHosts"))
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }
            app.UseRouting();
            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        private void ConfigureSqlServerContext(IServiceCollection services)
        {
            var connection = Configuration.GetConnectionString("DefaultConnection");
            if (IsInjectSqlServerConnectionString())
            {
                connection = $"{Environment.GetEnvironmentVariable("SQL_SERVER_CONNECTION")}";
            }

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    connection,
                    o => {
                        o.EnableRetryOnFailure(3);
                        o.CommandTimeout(3600);
                    }
                ).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            );
           
        }

        private static void AddVersioning(IServiceCollection services)
        {
            var apiVersioningBuilder = services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                // Use whatever reader you want
                options.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
                                                new HeaderApiVersionReader("x-api-version"),
                                                new MediaTypeApiVersionReader("x-api-version"));
            }); // Nuget Package: Asp.Versioning.Mvc

            apiVersioningBuilder.AddApiExplorer(options =>
            {
                // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                // note: the specified format code will format the version as "'v'major[.minor][-status]"
                options.GroupNameFormat = "'v'VVV";

                // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                // can also be used to control the format of the API version in route templates
                options.SubstituteApiVersionInUrl = true;
            }); // Nuget Package: Asp.Versioning.Mvc.ApiExplorer
        }
        private bool IsInjectSqlServerConnectionString()
        {
            return Configuration.GetValue<bool>("IsInjectSqlServerConnectionString");
        }
    }
}