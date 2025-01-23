namespace Feed.ML.FollowRecommendations
{
    public interface IFollowRecommendation
    {
        public Task<List<Guid>> GetFollowRecommendationsAsync(UserSuggestionQuery query);
    }
}
