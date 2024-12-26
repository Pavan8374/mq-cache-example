using Feed.API.Controllers;
using Feed.API.Manager.UserLikes;
using Feed.API.Models.Likes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Feed.API.Tests.Controllers
{
    [TestFixture]
    public class LikesControllerTests : ControllerTestsBase
    {
        private Mock<ILikeManager> _mockLikeManager;
        private Mock<ILogger<LikesController>> _mockLogger;
        private LikesController _controller;

        [SetUp]
        public void Setup()
        {
            _mockLikeManager = new Mock<ILikeManager>();
            _mockLogger = new Mock<ILogger<LikesController>>();
            _controller = new LikesController(_mockLikeManager.Object, _mockLogger.Object);
        }

        [Test]
        public async Task LikePost_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var request = new LikeRequest(UserId: Guid.NewGuid(), PostId: Guid.NewGuid());

            _mockLikeManager.Setup(x => x.HandleLikeAsync(request))
                           .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.LikePost(request);

            // Assert
            Assert.That(result, Is.InstanceOf<OkResult>());
            _mockLikeManager.Verify(x => x.HandleLikeAsync(request), Times.Once);
        }

        [Test]
        public async Task LikePost_WhenExceptionOccurs_Returns500Error()
        {
            // Arrange
            var request = new LikeRequest(UserId: Guid.NewGuid(),
                PostId: Guid.NewGuid());

            _mockLikeManager.Setup(x => x.HandleLikeAsync(request))
                           .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.LikePost(request);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Test]
        public async Task IsLiked_ValidRequest_ReturnsOkResultWithValue()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var postId = Guid.NewGuid();
            var expectedResult = true;
            _mockLikeManager.Setup(x => x.IsLikedAsync(userId, postId))
                           .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.IsLiked(userId, postId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo(expectedResult));
            _mockLikeManager.Verify(x => x.IsLikedAsync(userId, postId), Times.Once);
        }

        [Test]
        public async Task IsLiked_WhenExceptionOccurs_Returns500Error()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var postId = Guid.NewGuid();
            _mockLikeManager.Setup(x => x.IsLikedAsync(userId, postId))
                           .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.IsLiked(userId, postId);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Test]
        public async Task LikePost_VerifyCacheAndMessageQueue_HandledByManager()
        {
            // Arrange
            var request = new LikeRequest(UserId :Guid.NewGuid(),
                PostId : Guid.NewGuid());

            bool managerCalled = false;
            _mockLikeManager.Setup(x => x.HandleLikeAsync(request))
                           .Callback(() => managerCalled = true)
                           .Returns(Task.CompletedTask);

            // Act
            await _controller.LikePost(request);

            // Assert
            Assert.That(managerCalled, Is.True, "Like manager should be called to handle caching and message queue");
            _mockLikeManager.Verify(x => x.HandleLikeAsync(request), Times.Once);
        }
    }
}
