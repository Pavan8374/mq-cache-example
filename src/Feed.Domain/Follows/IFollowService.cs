using Feed.Domain.Base;

namespace Feed.Domain.Follows
{
    public interface IFollowService : IBaseService<Follow>
    {
        public Task<Follow> GetFollowRecord(Guid followerId, Guid followingId);

    }
}
