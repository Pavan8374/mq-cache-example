using Feed.Domain.Follows;
using Microsoft.EntityFrameworkCore;

namespace Feed.EF.Repositories
{
    public class FollowRepository : BaseRepository<Follow, ApplicationDbContext>, IFollowRepository
    {
        private readonly ApplicationDbContext _context;
        public FollowRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Follow> GetFollowRecord(Guid followerId, Guid followingId)
        {
            return await GetQuery().FirstOrDefaultAsync(x => x.FollowerId == followerId && x.FollowingId == followingId);
        }

    }
}
