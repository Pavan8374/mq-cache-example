using Feed.API.Configuration;
using Microsoft.AspNetCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Feed.API
{
    public class Program
    {
        /// <summary>
        /// The main entry point.
        /// </summary>
        /// <param name="args">Arguments.</param>
        public static void Main(string[] args)
        {
            try
            {
                //var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                var config = new ConfigurationBuilder()
                    .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    //.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                    .Build();



                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, "Fatal error occured, stopping application...");
            }
        }

        /// <summary>
        /// Creates and returns web host builder instance.
        /// </summary>
        /// <param name="args">Arguments.</param>
        /// <returns>Web host builder.</returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(builder =>
                {
                    var connectionString = GetSqlServerConnectionString(builder);
                    builder.AddDatabaseConfiguration(options => options.UseSqlServer(connectionString));
                })
                .UseStartup<StartUp>()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .ConfigureKestrel(t =>
                {
                    t.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(15);
                });
        }


        private static string GetSqlServerConnectionString(IConfigurationBuilder builder)
        {
            var connection = builder.Build().GetConnectionString("DefaultConnection");
            //var isInjectSqlServerConnectionString = builder.Build().GetValue<bool>("IsInjectSqlServerConnectionString");
            //if (isInjectSqlServerConnectionString)
            //    return $"{Environment.GetEnvironmentVariable("SQL_SERVER_CONNECTION")}{catalog}";

            return connection;
        }
    }
}

//var builder = WebApplication.CreateBuilder(args);
//ConfigurationManager configuration = builder.Configuration;

//var startup = new StartUp(configuration);
//// Add services to the container.

//builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));


//startup.ConfigureServices(builder.Services);


