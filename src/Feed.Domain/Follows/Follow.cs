using Feed.Domain.Base;
using Feed.Domain.Users;

namespace Feed.Domain.Follows
{
    public class Follow : BaseEntity
    {
        public Guid FollowerId { get; set; }
        public Guid FollowingId { get; set; }
        public virtual User Follower{ get; set; }
        public virtual User Following{ get; set; }
    }
}
