using Feed.API.Models.Likes;

namespace Feed.API.Manager.UserLikes
{
    public interface ILikeManager 
    {
        public Task HandleLikeAsync(LikeRequest request);
        public Task<bool> IsLikedAsync(Guid userId, Guid postId);
    }


}
