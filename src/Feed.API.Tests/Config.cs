using Feed.API.Controllers;
using Feed.API.Manager.UserLikes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Feed.API.Tests
{
    [SetUpFixture]
    public class Config
    {
        public static ILikeManager likeManager;
        public static ILogger<LikesController> logger;
        public static HttpClient httpClient;

        [OneTimeSetUp]
        public static void SetUpFixture()
        {
            var builder = new WebHostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {"RabbitMQSettings:HostName", "localhost"},
                        {"RabbitMQSettings:UserName", "guest"},
                        {"RabbitMQSettings:Password", "guest"},
                        {"RabbitMQSettings:VirtualHost", "/"},
                        {"RabbitMQSettings:Port", "5672"},
                        {"Redis:ConnectionString", "localhost:6379"}
                    });
                })
                .ConfigureServices(services =>
                {
                    services.AddLogging(logging =>
                    {
                        logging.AddConsole();
                        logging.SetMinimumLevel(LogLevel.Debug);
                    });
                })
                .UseStartup<StartupStub>();

            var server = new TestServer(builder);

            // Resolve dependencies
            likeManager = server.Host.Services.GetRequiredService<ILikeManager>();
            logger = server.Host.Services.GetRequiredService<ILogger<LikesController>>();
            httpClient = server.CreateClient();
        }

        [OneTimeTearDown]
        public static void TearDownFixture()
        {
            httpClient.Dispose();
        }
    }
}