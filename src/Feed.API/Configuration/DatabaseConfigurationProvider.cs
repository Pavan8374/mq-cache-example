using Feed.EF;
using Microsoft.EntityFrameworkCore;

namespace Feed.API.Configuration
{
    public class DatabaseConfigurationProvider : ConfigurationProvider
    {
        private readonly Action<DbContextOptionsBuilder<ApplicationDbContext>> _options;

        /// <summary>
        /// Creates database configuration provider.
        /// </summary>
        /// <param name="options">Database context options.</param>
        public DatabaseConfigurationProvider(
            Action<DbContextOptionsBuilder<ApplicationDbContext>> options)
        {
            _options = options;
        }

        
    }
}
