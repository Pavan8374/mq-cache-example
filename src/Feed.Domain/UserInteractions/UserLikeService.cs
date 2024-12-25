using Feed.Domain.Base;

namespace Feed.Domain.UserInteractions
{
    public class UserLikeService : BaseService<UserLike>, IUserLikeService
    {
        private readonly IUserLikeRepository _userLikeRepository;
        public UserLikeService(IUserLikeRepository userLikeRepository) : base(userLikeRepository)
        {
            _userLikeRepository = userLikeRepository;
        }
    }
}
