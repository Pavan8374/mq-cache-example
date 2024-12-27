using Feed.EF;
using Microsoft.EntityFrameworkCore;

namespace Feed.API.Configuration
{
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// Adds database configuration source.
        /// </summary>
        /// <param name="builder">Configuration builder.</param>
        /// <param name="setup">Database context setup.</param>
        /// <returns>Configuration builder.</returns>
        public static IConfigurationBuilder AddDatabaseConfiguration(
            this IConfigurationBuilder builder, Action<DbContextOptionsBuilder<ApplicationDbContext>> setup)
        {
            return builder.Add(new DatabaseConfigurationSource(setup));
        }
    }
}
