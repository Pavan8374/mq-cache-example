using Feed.Domain.UserInteractions;

namespace Feed.EF.Repositories
{
    public class UserLikeRepository : BaseRepository<UserLike, ApplicationDbContext>, IUserLikeRepository
    {
        private readonly ApplicationDbContext _context;
        public UserLikeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
