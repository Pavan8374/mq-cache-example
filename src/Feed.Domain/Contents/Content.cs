using Feed.Domain.Base;
using Feed.Domain.Users;

namespace Feed.Domain.Contents
{
    public class Content : BaseEntity
    {
        public string Description { get; set; }
        public long LikesCount { get; set; }
        public Guid UserId { get; set; }
        public virtual User User { get; set; }
    }
}
