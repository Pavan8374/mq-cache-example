using Feed.Domain.Follows;
using Feed.Domain.Users;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;

namespace Feed.ML.FollowRecommendations
{
    
    public class FollowRecommendation : IFollowRecommendation
    {
        private readonly IUserRepository  _userRepository;
        private readonly IFollowRepository  _followRepository;
        private readonly MLContext _mlContext;
        public FollowRecommendation(IUserRepository userRepository, IFollowRepository followRepository)
        {
            _userRepository = userRepository;
            _followRepository = followRepository;
            _mlContext = new MLContext();
        }
        

        /// <summary>
        /// Get follow recommendations based on proximity and mutual connections.
        /// </summary>
        /// <param name="query">User suggestion query containing user details.</param>
        /// <returns>List of recommended user IDs.</returns>
        public async Task<List<Guid>> GetFollowRecommendationsAsync(UserSuggestionQuery query)
        {
            try
            {
                // Step 1: Prepare the follow data
                var followData = await PrepareFollowData();

                // Step 2: Get nearby users
                var nearbyUsers = GetNearbyUsers(query);

                // Step 3: Filter follow data to include only nearby users
                var filteredFollowData = followData
                    .Where(fd => nearbyUsers.Contains(Guid.Parse(fd.TargetUserId)))
                    .ToList();

                // Step 4: Prepare training data
                var trainData = PrepareTrainData(filteredFollowData);

                // Step 5: Train the model
                var model = TrainModel(trainData);

                // Step 6: Predict recommendations for the user
                var recommendedUsers = GetRecommendations(model, query.UserId, nearbyUsers);

                return recommendedUsers;
            }
            catch (Exception)
            {

                throw;
            }
                
        }

        private IDataView PrepareTrainData(List<FollowData> followData)
        {
            var schemaDefinition = SchemaDefinition.Create(typeof(FollowData));
            schemaDefinition[nameof(FollowData.UserId)].ColumnType = TextDataViewType.Instance;
            schemaDefinition[nameof(FollowData.TargetUserId)].ColumnType = TextDataViewType.Instance;

            // Explicitly specify the type argument for LoadFromEnumerable
            return _mlContext.Data.LoadFromEnumerable<FollowData>(followData, schemaDefinition);
        }

        private ITransformer TrainModel(IDataView trainData)
        {
            try
            {
                // Define the pipeline
                var pipeline = _mlContext.Recommendation().Trainers.MatrixFactorization(new MatrixFactorizationTrainer.Options
                {
                    MatrixColumnIndexColumnName = nameof(FollowData.UserIdEncoded),
                    MatrixRowIndexColumnName = nameof(FollowData.TargetUserIdEncoded),
                    LabelColumnName = nameof(FollowData.Rating),
                    NumberOfIterations = 20,
                    ApproximationRank = 100
                });

                // Encode the UserId and TargetUserId as keys for Matrix Factorization
                var preprocessingPipeline = _mlContext.Transforms.Conversion
                    .MapValueToKey(nameof(FollowData.UserId), nameof(FollowData.UserIdEncoded))
                    .Append(_mlContext.Transforms.Conversion.MapValueToKey(nameof(FollowData.TargetUserId), nameof(FollowData.TargetUserIdEncoded)));

                var fullPipeline = preprocessingPipeline.Append(pipeline);

                // Train the model
                var model = fullPipeline.Fit(trainData);

                return model;
            }
            catch (Exception)
            {

                throw;
            }

            
        }

        private List<Guid> GetRecommendations(ITransformer model, Guid userId, HashSet<Guid> nearbyUsers)
        {
            // Prepare prediction engine
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<FollowData, FollowPrediction>(model);

            // Generate predictions for nearby users
            var predictions = nearbyUsers
                .Select(targetUserId =>
                {
                    var prediction = predictionEngine.Predict(new FollowData
                    {
                        UserId = userId.ToString(),
                        TargetUserId = targetUserId.ToString()
                    });
                    return new { TargetUserId = targetUserId, Score = prediction.Score };
                })
                .OrderByDescending(p => p.Score) // Sort by score
                .Take(10) // Return top 10 recommendations
                .Select(p => p.TargetUserId)
                .ToList();

            return predictions;
        }

        private async Task<List<FollowData>> PrepareFollowData()
        {
            // Collect follower-following data from the repository
            var follows = await  _followRepository.GetAll(); // Assume this returns all follows as a list.

            return follows.Select(f => new FollowData
            {
                UserId = f.FollowerId.ToString(),
                TargetUserId = f.FollowingId.ToString(),
                Rating = 1 // Implicit feedback: follow action as a positive rating.
            }).ToList();
        }


        private HashSet<Guid> GetNearbyUsers(UserSuggestionQuery query)
        {
            const double proximityLimit = 10.0; // 10 km proximity limit
            var userLocations = _userRepository.GetUserLocationsOptimized();

            return userLocations
                .Where(u => u.Key != query.UserId && // Exclude self
                            CalculateDistance(query.Latitude, query.Longitude, u.Value.Latitude, u.Value.Longitude) <= proximityLimit)
                .Select(u => u.Key)
                .ToHashSet();
        }

        /// <summary>
        /// Calculate distance using the Haversine formula.
        /// </summary>
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Radius of Earth in km.
            var dLat = (lat2 - lat1) * (Math.PI / 180);
            var dLon = (lon2 - lon1) * (Math.PI / 180);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(lat1 * (Math.PI / 180)) * Math.Cos(lat2 * (Math.PI / 180)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c; // Distance in km.
        }



        public class FollowData
        {
            [LoadColumn(0)]
            public string UserId { get; set; }

            [LoadColumn(1)]
            public string TargetUserId { get; set; }

            [LoadColumn(2)]
            public float Rating { get; set; } = 1;

            // Change to use Key<uint> type
            [KeyType(count: 1000)] // Specify an appropriate max count
            public uint UserIdEncoded { get; set; }

            [KeyType(count: 1000)] // Specify an appropriate max count
            public uint TargetUserIdEncoded { get; set; }
        }

        public class FollowPrediction
        {
            public float Score { get; set; }
        }
    }
}
