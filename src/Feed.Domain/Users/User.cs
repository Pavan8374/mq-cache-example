using Feed.Domain.Base;
using Feed.Domain.Follows;

namespace Feed.Domain.Users
{
    public class User : BaseEntity
    {
        public string UserName{ get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PasswordHash { get; set; }
        public string? Salt { get; set; }
        public string? FullName { get; set; }
        public long Longitude { get; set; }
        public long Latitude { get; set; }
        public virtual IEnumerable<Follow> Followers { get; set; }
        public virtual IEnumerable<Follow> Followings { get; set; }
    }
}
