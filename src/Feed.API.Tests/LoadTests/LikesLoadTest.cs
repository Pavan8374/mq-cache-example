using Feed.API.Controllers;
using Feed.API.Manager.UserLikes;
using Feed.API.Models.Likes;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;

namespace Feed.API.Tests.LoadTests
{
    [TestFixture]
    public class LikesLoadTest : ControllerTestsBase
    {
        private LikesController _controller;
        private ILikeManager _likeManager;
        private ILogger<LikesController> _logger;
        private const int NUMBER_OF_USERS = 1000;
        private const int POSTS_PER_USER = 10;
        private const int CONCURRENT_REQUESTS = 100;

        [SetUp]
        public void Setup()
        {
            // Use your actual dependency injection container or manual instantiation
            _likeManager = Config.likeManager;   // Your actual like manager instance
            _logger = Config.logger;
            _controller = new LikesController(_likeManager, _logger);
        }

        [Test]
        public async Task GenerateHighTrafficLoad()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var users = GenerateUsers(NUMBER_OF_USERS);
            var posts = GeneratePosts(POSTS_PER_USER);
            var random = new Random();

            Console.WriteLine($"Starting load test with {NUMBER_OF_USERS} users and {POSTS_PER_USER} posts per user");
            Console.WriteLine($"Total potential likes: {NUMBER_OF_USERS * POSTS_PER_USER}");

            var tasks = new List<Task>();
            var successCount = 0;
            var failureCount = 0;
            var semaphore = new SemaphoreSlim(CONCURRENT_REQUESTS);

            foreach (var userId in users)
            {
                foreach (var postId in posts)
                {
                    await semaphore.WaitAsync();

                    tasks.Add(Task.Run(async () =>
                    {
                        try
                        {
                            var request = new LikeRequest(UserId: userId,
                             PostId: postId);

                            // Add some randomization to make it more realistic
                            if (random.Next(100) < 80) // 80% chance to like
                            {
                                var response = await HttpClient.PostAsJsonAsync("/api/Likes", request);

                                if(response.IsSuccessStatusCode)
                                    Interlocked.Increment(ref successCount);
                                else
                                    Interlocked.Increment(ref failureCount);

                                //await _controller.LikePost(request);

                                // Randomly check like status
                                if (random.Next(100) < 30) // 30% chance to check status
                                {
                                    await HttpClient.PostAsJsonAsync($"/api/Likes/{userId}/{postId}", request);
                                    //await _controller.IsLiked(userId, postId);
                                }
                            }

                            // Add random delay between 0-100ms to simulate network variance
                            await Task.Delay(random.Next(100));
                        }
                        catch (Exception ex)
                        {
                            Interlocked.Increment(ref failureCount);
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }));

                    // Print progress every 1000 requests
                    if ((successCount + failureCount) % 1000 == 0)
                    {
                        Console.WriteLine($"Processed {successCount + failureCount} requests. " +
                                        $"Success: {successCount}, Failures: {failureCount}");
                    }
                }
            }

            await Task.WhenAll(tasks);
            stopwatch.Stop();

            Console.WriteLine("\nLoad Test Summary:");
            Console.WriteLine($"Total Time: {stopwatch.ElapsedMilliseconds / 1000.0:F2} seconds");
            Console.WriteLine($"Successful Requests: {successCount}");
            Console.WriteLine($"Failed Requests: {failureCount}");
            Console.WriteLine($"Requests per Second: {(successCount + failureCount) / (stopwatch.ElapsedMilliseconds / 1000.0):F2}");
        }

        [Test]
        public async Task SimulateBurstTraffic()
        {
            // Simulate sudden burst of traffic (e.g., viral post)
            const int BURST_SIZE = 5000;
            const int CONCURRENT_BURST = 200;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var postId = Guid.NewGuid(); // Single post receiving burst of likes
            var users = GenerateUsers(BURST_SIZE);
            var semaphore = new SemaphoreSlim(CONCURRENT_BURST);
            var tasks = new List<Task>();
            var successCount = 0;
            var failureCount = 0;

            Console.WriteLine($"Starting burst test with {BURST_SIZE} users liking the same post");

            foreach (var userId in users)
            {
                await semaphore.WaitAsync();

                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        var request = new LikeRequest(UserId: userId,
                            PostId: postId);

                        await _controller.LikePost(request);
                        Interlocked.Increment(ref successCount);
                    }
                    catch (Exception ex)
                    {
                        Interlocked.Increment(ref failureCount);
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }

            await Task.WhenAll(tasks);
            stopwatch.Stop();

            Console.WriteLine("\nBurst Test Summary:");
            Console.WriteLine($"Total Time: {stopwatch.ElapsedMilliseconds / 1000.0:F2} seconds");
            Console.WriteLine($"Successful Requests: {successCount}");
            Console.WriteLine($"Failed Requests: {failureCount}");
            Console.WriteLine($"Requests per Second: {(successCount + failureCount) / (stopwatch.ElapsedMilliseconds / 1000.0):F2}");
        }

        private List<Guid> GenerateUsers(int count)
        {
            var users = new List<Guid>();
            for (int i = 0; i < count; i++)
            {
                users.Add(Guid.NewGuid());
            }
            return users;
        }

        private List<Guid> GeneratePosts(int count)
        {
            var posts = new List<Guid>();
            for (int i = 0; i < count; i++)
            {
                posts.Add(Guid.NewGuid());
            }
            return posts;
        }

        
    }
}