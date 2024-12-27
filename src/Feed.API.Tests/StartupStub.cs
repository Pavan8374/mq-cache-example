using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Feed.API.Tests
{
    public class StartupStub : StartUp
    {
        public StartupStub(IConfiguration configuration)
            : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
        }
    }
}
