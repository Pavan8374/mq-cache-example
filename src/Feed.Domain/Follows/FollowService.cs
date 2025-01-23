using Feed.Domain.Base;

namespace Feed.Domain.Follows
{
    public class FollowService : BaseService<Follow>, IFollowService
    {
        private readonly IFollowRepository _followRepository;
        public FollowService(IFollowRepository followRepository) : base(followRepository)
        {
            _followRepository = followRepository;
        }

        public async Task<Follow> GetFollowRecord(Guid followerId, Guid followingId)
        {
            return await _followRepository.GetFollowRecord(followerId, followingId);
        }

    }
}
