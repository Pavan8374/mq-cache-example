using Feed.EF;
using Microsoft.EntityFrameworkCore;

namespace Feed.API.Tests
{
    [TestFixture]
    public abstract class ControllerTestsBase
    {
        protected HttpClient HttpClient;
        protected ApplicationDbContext dbContext;

        [SetUp]
        public virtual async Task SetUpAsync()
        {
            HttpClient = Config.httpClient;
            dbContext = Config.dbContext;
        }

        protected ApplicationDbContext CreateDbContext()
        {
            return Config.CreateDbContext();
        }
    }
}
