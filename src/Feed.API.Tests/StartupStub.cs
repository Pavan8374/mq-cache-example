using Microsoft.Extensions.Configuration;

namespace Feed.API.Tests
{
    public class StartupStub : StartUp
    {
        public StartupStub() : base(new ConfigurationBuilder().Build())
        {
        }
    }
}
