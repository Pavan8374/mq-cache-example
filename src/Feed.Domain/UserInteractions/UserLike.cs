using Feed.Domain.Base;

namespace Feed.Domain.UserInteractions
{
    public class UserLike : BaseEntity
    {
        public Guid ContentId{ get; set; }
        public Guid UserId{ get; set; }
        //public virtual Content Content{ get; set; }
        //public virtual User Uesr{ get; set; }
    }
}
