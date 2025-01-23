using Feed.Domain.Base;

namespace Feed.Domain.Users
{
    public interface IUserService : IBaseService<User>
    {
        public Task SeedDummyData(int userCount, int maxFollowsPerUser, double centerLatitude, double centerLongitude);
    }
}
