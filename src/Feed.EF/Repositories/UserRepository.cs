using Bogus;
using Feed.Domain.Follows;
using Feed.Domain.Users;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace Feed.EF.Repositories
{
    public class UserRepository : BaseRepository<User, ApplicationDbContext>, IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Get users as dictionary containing id as key, (lat, long) as value pairs.
        /// </summary>
        /// <returns>Immutable dictinary of users and (Lat, long) key value pairs.</returns>
        public  ImmutableDictionary<Guid, (long Latitude, long Longitude)> GetUserLocationsOptimized()
        {
            return  _context.Users
                .AsNoTracking() // Reduces EF Core overhead
                .Select(u => new
                {
                    UserId = u.Id,
                    Latitude = u.Latitude,
                    Longitude = u.Longitude
                })
                .ToImmutableDictionary(
                    u => u.UserId,
                    u => (u.Latitude, u.Longitude)
                );
        }

        /// <summary>
        /// Get mutual connections for a user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>List of mutual connections.</returns>
        public List<Guid> GetMutualConnections(Guid userId)
        {
            // Query mutual connections in a single efficient query.
            return _context.Follows
                .AsNoTracking()
                .Where(f => f.FollowerId == userId || f.FollowingId == userId) // Get connections.
                .GroupBy(f => f.FollowerId == userId ? f.FollowingId : f.FollowerId)
                .Where(g => g.Count() > 1) // Ensure mutual.
                .Select(g => g.Key) // Extract mutual connection IDs.
                .ToList();
        }



        public async Task SeedDummyData(int userCount, int maxFollowsPerUser, double centerLatitude, double centerLongitude)
        {
            var faker = new Faker();
            var users = new List<User>();

            // Generate users with random coordinates within 10 km radius
            for (int i = 0; i < userCount; i++)
            {
                // Generate random latitude and longitude within 10 km radius
                double radiusInKm = 10;
                double latOffset = (faker.Random.Double() * 2 - 1) * (radiusInKm / 110.574); // Latitude offset
                double lonOffset = (faker.Random.Double() * 2 - 1) * (radiusInKm / (111.320 * Math.Cos(centerLatitude * Math.PI / 180))); // Longitude offset

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    UserName = faker.Internet.UserName(),
                    Email = faker.Internet.Email(),
                    PhoneNumber = faker.Phone.PhoneNumber(),
                    PasswordHash = faker.Internet.Password(),
                    Salt = faker.Random.String2(10),
                    FullName = faker.Name.FullName(),
                    Longitude = (long)(centerLongitude + lonOffset),
                    Latitude = (long)(centerLatitude + latOffset),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                users.Add(user);
            }

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync(); // Save users to get their IDs

            // Generate follows
            var follows = new List<Follow>();
            var userIds = users.Select(u => u.Id).ToList();

            foreach (var user in users)
            {
                // Randomly determine how many follows this user will have
                int followsCount = faker.Random.Int(1, maxFollowsPerUser);

                // Get unique followers for this user
                var followedUsers = faker.Random.ListItems(userIds.Where(id => id != user.Id).ToList(), followsCount);

                foreach (var followingId in followedUsers)
                {
                    follows.Add(new Follow
                    {
                        Id = Guid.NewGuid(),
                        FollowerId = user.Id,
                        FollowingId = followingId,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.Follows.AddRangeAsync(follows);
            await _context.SaveChangesAsync(); // Save follows
        }



    }
}
