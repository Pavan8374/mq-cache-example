using Feed.API.Authorization;
using Feed.API.Controllers;
using Feed.API.Manager.UserLikes;
using Feed.EF;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Feed.API.Tests
{
    [SetUpFixture]
    public class Config
    {
        private static IConfiguration _configuration;
        public static ILikeManager likeManager;
        public static ILogger<LikesController> logger;
        public static HttpClient httpClient;
        public static ApplicationDbContext dbContext;

        [OneTimeSetUp]
        public static void SetUpFixture()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            dbContext = CreateDbContext();

            var server = new TestServer(new WebHostBuilder()
                .UseConfiguration(_configuration)
                .UseStartup<StartupStub>());


            //var builder = new WebHostBuilder()
            //    .UseConfiguration(_configuration)
            //    .ConfigureAppConfiguration((hostingContext, config) =>
            //    {
            //        config.AddInMemoryCollection(new Dictionary<string, string>
            //        {
            //            {"RabbitMQSettings:HostName", "localhost"},
            //            {"RabbitMQSettings:UserName", "guest"},
            //            {"RabbitMQSettings:Password", "guest"},
            //            {"RabbitMQSettings:VirtualHost", "/"},
            //            {"RabbitMQSettings:Port", "5672"},
            //            {"Redis:ConnectionString", "localhost:6379"}
            //        });
            //    })
            //    .ConfigureServices(services =>
            //    {
            //        services.AddLogging(logging =>
            //        {
            //            logging.AddConsole();
            //            logging.SetMinimumLevel(LogLevel.Debug);
            //        });
            //    })
            //    .UseStartup<StartupStub>()
            //    //.UseUrls("https://localhost:44325")
            //    ;


            httpClient = server.CreateClient();
            //httpClient.BaseAddress = new Uri("https://localhost:44325/");

            // Resolve dependencies
            likeManager = server.Host.Services.GetRequiredService<ILikeManager>();
            logger = server.Host.Services.GetRequiredService<ILogger<LikesController>>();
            
        }
        //protected virtual void AddAuthorization(IServiceCollection services)
        //{
            

        //    var policies = typeof(ApplicationServerPolicies)
        //        .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
        //        .Where(x => x.IsLiteral && !x.IsInitOnly)
        //        .Select(x => x.GetRawConstantValue().ToString())
        //        .ToList();

        //    services
        //        .AddAuthorization(options =>
        //        {
        //            foreach (var policy in policies)
        //            {
        //                options.AddPolicy(policy, builder => builder.RequireRole("test"));
        //            }
        //        });
        //}
        public static ApplicationDbContext CreateDbContext()
        {
            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .EnableSensitiveDataLogging()
                .UseSqlServer(_configuration.GetConnectionString("DefaultConnection"))
                .Options;

            return new ApplicationDbContext(dbContextOptions);
        }

        [OneTimeTearDown]
        public static void TearDownFixture()
        {
            httpClient.Dispose();
            dbContext.Dispose();
        }   
    }
}