using Feed.Domain.Base;

namespace Feed.Domain.Users
{
    public class UserService : BaseService<User>, IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository) : base(userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task SeedDummyData(int userCount, int maxFollowsPerUser, double centerLatitude, double centerLongitude)
        {
            await _userRepository.SeedDummyData(userCount, maxFollowsPerUser, centerLatitude, centerLongitude);
        }


    }
}
