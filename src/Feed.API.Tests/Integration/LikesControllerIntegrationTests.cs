using Azure.Core;
using Feed.API.Controllers;
using Feed.API.Models.Likes;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using NBomber.CSharp;
using System.Net;
using System.Net.Http.Json;

namespace Feed.API.Tests.Integration
{
    [TestFixture]
    public class LikesControllerIntegrationTests : ControllerTestsBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private static readonly Random _random = new Random();
        private LikesController _controller;

        public LikesControllerIntegrationTests()
        {
            _httpClient = Config.httpClient;
            _baseUrl = "/v1/Likes";

            _controller = new LikesController(Config.likeManager, Config.logger);
        }

        [Test]
        public async Task ParallelLikeRequests_ShouldHandleHighLoad()
        {
            // Arrange
            int numberOfUsers = 1000;
            int numberOfPosts = 100;
            var users = Enumerable.Range(0, numberOfUsers).Select(_ => Guid.NewGuid()).ToList();
            var posts = Enumerable.Range(0, numberOfPosts).Select(_ => Guid.NewGuid()).ToList();

            var likeRequests = GenerateRandomLikeRequests(users, posts, 10000);

            // Act
            var tasks = likeRequests.Select(async request =>
            {
                var response = await _controller.LikePost(request);

                var result = response as ObjectResult;

                Console.WriteLine(result?.Value);
                return result.StatusCode.HasValue;
                
            });

            var results = await Task.WhenAll(tasks);

            // Assert
            results.Count(x => x).Should().Be(likeRequests.Count);
        }

        [Test]
        public async Task LikePost_ShouldReturnOk()
        {
            var userId = Guid.NewGuid();
            var postId = Guid.NewGuid();
            var likeRequest = new LikeRequest(userId, postId);

            //var request = new Dictionary<string, string>
            //{
            //    { "userId", userId.ToString() },
            //    { "postId", postId.ToString() },
            //    //{ "grant_type", "password" }
            //};
            //var reqContent = new FormUrlEncodedContent(request);

            //var response = await HttpClient.PostAsync($"/v1/Likes", reqContent);

            var response = await _controller.LikePost(likeRequest);

            //response.EnsureSuccessStatusCode();

            //response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            //var content = response.Content.ReadAsStringAsync();

            Console.WriteLine($"Response Status: {response}");
            //Console.WriteLine($"Response Content: {content}");

            // Assert
            //response.StatusCode.Should().Be(HttpStatusCode.OK);
            //content.Should().Be("User liked the post");
        }

        [Test]
        public async Task ConcurrentLikesAndChecks_ShouldMaintainConsistency()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var postId = Guid.NewGuid();
            int numberOfOperations = 100;

            // Act
            var tasks = new List<Task>();
            for (int i = 0; i < numberOfOperations; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    // Like the post
                    await _httpClient.PostAsJsonAsync($"{_baseUrl}", new LikeRequest(UserId : userId, PostId :postId));

                    // Check if liked
                    var response = await _httpClient.GetAsync($"{_baseUrl}/{userId}/{postId}");
                    var result = await response.Content.ReadFromJsonAsync<bool>();
                    return result;
                }));
            }

            await Task.WhenAll(tasks);

            // Assert
            //results.All(x => x).Should().BeTrue();
        }

        [Test]
        public void LoadTest_ShouldHandleHighThroughput()
        {
            // Arrange
            var scenario = Scenario.Create("like_posts_scenario", async context =>
            {
                var request = new LikeRequest(UserId: Guid.NewGuid(), PostId: Guid.NewGuid()
                );

                // Act
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}", request);

                // Assert
                return response.IsSuccessStatusCode
                    ? NBomber.CSharp.Response.Ok()
                    : NBomber.CSharp.Response.Fail();
            })
            .WithLoadSimulations(
                Simulation.Inject(rate: 100,
                                interval: TimeSpan.FromSeconds(1),
                                during: TimeSpan.FromMinutes(1))
            );

            // Configure NBomber test
            var stats = NBomberRunner
                .RegisterScenarios(scenario)
                .WithTestSuite("Likes API Load Test")
                .WithTestName("High Throughput Test")
                .Run();

            // Assert
            stats.ScenarioStats[0].Ok.Request.RPS.Should().BeGreaterThan(50);
            stats.ScenarioStats[0].Fail.Request.Count.Should().Be(0);
        }

        [Test]
        public async Task CacheConsistency_UnderLoad_ShouldMaintainAccuracy()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var postId = Guid.NewGuid();
            int numberOfOperations = 1000;

            // Act
            // First, like the post
            await _httpClient.PostAsJsonAsync($"{_baseUrl}", new LikeRequest(UserId: userId, PostId: postId));


            // Then perform multiple concurrent check operations
            var tasks = Enumerable.Range(0, numberOfOperations).Select(_ =>
                _httpClient.GetAsync($"{_baseUrl}/{userId}/{postId}"));

            var responses = await Task.WhenAll(tasks);
            var results = await Task.WhenAll(
                responses.Select(r => r.Content.ReadFromJsonAsync<bool>()));

            // Assert
            results.All(x => x).Should().BeTrue();
        }

        private List<LikeRequest> GenerateRandomLikeRequests(List<Guid> users, List<Guid> posts, int count)
        {
            var requests = new List<LikeRequest>();
            for (int i = 0; i < count; i++)
            {
                requests.Add(new LikeRequest(
                
                    UserId :users[_random.Next(users.Count)],
                    PostId : posts[_random.Next(posts.Count)]
                ));
            }
            return requests;
        }

        [TearDown]
        public void Cleanup()
        {
           //HttpClient.Dispose();
        }
    }
}