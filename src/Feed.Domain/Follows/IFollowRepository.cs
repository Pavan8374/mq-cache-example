using Feed.Domain.Base;

namespace Feed.Domain.Follows
{
    public interface IFollowRepository : IBaseRepository<Follow>
    {
        public Task<Follow> GetFollowRecord(Guid followerId, Guid followingId);
    }
}
