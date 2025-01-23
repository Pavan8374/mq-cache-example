using Feed.Domain.Base;
using System.Collections.Immutable;

namespace Feed.Domain.Users
{
    public interface IUserRepository : IBaseRepository<User>
    {
        /// <summary>
        /// Get users as dictionary containing id as key, (lat, long) as value pairs.
        /// </summary>
        /// <returns>Immutable dictinary of users and (Lat, long) key value pairs.</returns>
        public ImmutableDictionary<Guid, (long Latitude, long Longitude)> GetUserLocationsOptimized();

        /// <summary>
        /// Get mutual connections for a user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>List of mutual connections.</returns>
        public List<Guid> GetMutualConnections(Guid userId);

        public Task SeedDummyData(int userCount, int maxFollowsPerUser, double centerLatitude, double centerLongitude);

    }
}
