namespace Feed.API.Tests
{
    [TestFixture]
    public abstract class ControllerTestsBase
    {
        protected HttpClient HttpClient;

        [SetUp]
        public virtual async Task SetUpAsync()
        {
            HttpClient = Config.httpClient;
        }
    }
}
