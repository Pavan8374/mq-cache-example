namespace Feed.ML.FollowRecommendations
{
    public class UserSuggestionQuery
    {
        public Guid UserId { get; set; }
        public long Latitude { get; set; }
        public long Longitude { get; set; }
    }
}
