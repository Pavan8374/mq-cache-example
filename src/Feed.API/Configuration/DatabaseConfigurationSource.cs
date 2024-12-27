using Feed.EF;
using Microsoft.EntityFrameworkCore;

namespace Feed.API.Configuration
{
    public class DatabaseConfigurationSource : IConfigurationSource
    {
        private readonly Action<DbContextOptionsBuilder<ApplicationDbContext>> _options;

        /// <summary>
        /// Creates database configuration source.
        /// </summary>
        /// <param name="options">Database context options.</param>
        public DatabaseConfigurationSource(Action<DbContextOptionsBuilder<ApplicationDbContext>> options)
        {
            _options = options;
        }

        /// <summary>
        /// Returns configuration provider.
        /// </summary>
        /// <param name="builder">Configuration builder.</param>
        /// <returns>Configuration provider.</returns>
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new DatabaseConfigurationProvider(_options);
        }
    }
}
