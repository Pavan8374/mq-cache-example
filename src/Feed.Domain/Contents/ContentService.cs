using Feed.Domain.Base;

namespace Feed.Domain.Contents
{
    public class ContentService : BaseService<Content>, IContentService
    {
        private readonly IContentRepository _contentRepository;
        public ContentService(IContentRepository contentRepository) : base(contentRepository)
        {
            _contentRepository = contentRepository;
        }
    }
}
