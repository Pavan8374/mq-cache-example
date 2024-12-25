using Feed.Domain.Contents;

namespace Feed.EF.Repositories
{
    public class ContentRepository : BaseRepository<Content, ApplicationDbContext>, IContentRepository
    {
        private readonly ApplicationDbContext _context;
        public ContentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
