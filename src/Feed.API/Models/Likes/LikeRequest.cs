namespace Feed.API.Models.Likes
{
    public record LikeRequest(Guid UserId, Guid PostId);
}
