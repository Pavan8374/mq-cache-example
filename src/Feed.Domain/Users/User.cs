using Feed.Domain.Base;

namespace Feed.Domain.Users
{
    public class User : BaseEntity
    {
        public Guid UserName{ get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PasswordHash { get; set; }
        public string? Salt { get; set; }
        public string? FullName { get; set; }
    }
}
