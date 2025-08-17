using GeocitiesTimes.Server.Controllers;
using GeocitiesTimes.Server.Clients;
using GeocitiesTimes.Server.Models;
using GeocitiesTimes.Server.Providers.Pages;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace GeocitiesTimes.Server.Tests.Controllers
{
    [TestFixture]
    public class StoryControllerTests
    {
        private Mock<IPagesProvider> _mockPagesProvider;
        private Mock<INewsClient> _mockNewsClient;
        private StoryController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockPagesProvider = new Mock<IPagesProvider>();
            _mockNewsClient = new Mock<INewsClient>();
            _controller = new StoryController(_mockPagesProvider.Object, _mockNewsClient.Object);
        }

        [Test]
        public async Task GetNewStories_ValidRequest_ReturnsOkWithStories()
        {
            // Arrange
            var dto = new NewsRequestDTO
            {
                PageNum = 1,
                PageSize = 10,
                SearchTerm = "test"
            };

            var storyIds = new[] { 1, 2, 3 };
            var stories = new List<IEnumerable<Story>>
            {
                new List<Story>
                {
                    new Story { Id = 1, Title = "Test Story 1" },
                    new Story { Id = 2, Title = "Test Story 2" }
                }
            };

            _mockNewsClient.Setup(x => x.GetNewStoryIds())
                          .ReturnsAsync(storyIds);

            _mockPagesProvider.Setup(x => x.GetStoryPages(storyIds, dto.PageNum, dto.PageSize, dto.SearchTerm))
                             .Returns(Task.FromResult<IEnumerable<IEnumerable<Story>>>(stories));

            // Act
            var result = await _controller.GetNewStories(dto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<IEnumerable<Story>>>());
            var returnedStories = (IEnumerable<IEnumerable<Story>>)okResult.Value;
            Assert.That(returnedStories, Is.EqualTo(stories));

            _mockNewsClient.Verify(x => x.GetNewStoryIds(), Times.Once);
            _mockPagesProvider.Verify(x => x.GetStoryPages(storyIds, dto.PageNum, dto.PageSize, dto.SearchTerm), Times.Once);
        }

        [Test]
        public async Task GetNewStories_NoStoriesFound_ReturnsNotFound()
        {
            // Arrange
            var dto = new NewsRequestDTO
            {
                PageNum = 1,
                PageSize = 10,
                SearchTerm = "nonexistent"
            };

            var storyIds = new[] { 1, 2, 3 };
            var emptyStories = new List<IEnumerable<Story>>();

            _mockNewsClient.Setup(x => x.GetNewStoryIds())
                          .ReturnsAsync(storyIds);

            _mockPagesProvider.Setup(x => x.GetStoryPages(storyIds, dto.PageNum, dto.PageSize, dto.SearchTerm))
                             .Returns(Task.FromResult<IEnumerable<IEnumerable<Story>>>(emptyStories));

            // Act
            var result = await _controller.GetNewStories(dto);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());

            _mockNewsClient.Verify(x => x.GetNewStoryIds(), Times.Once);
            _mockPagesProvider.Verify(x => x.GetStoryPages(storyIds, dto.PageNum, dto.PageSize, dto.SearchTerm), Times.Once);
        }

        [Test]
        public async Task GetNewStories_WithNullSearchTerm_ReturnsOkWithStories()
        {
            // Arrange
            var dto = new NewsRequestDTO
            {
                PageNum = 1,
                PageSize = 10,
                SearchTerm = null
            };

            var storyIds = new[] { 1, 2, 3 };
            var stories = new List<IEnumerable<Story>>
            {
                new List<Story>
                {
                    new Story { Id = 1, Title = "Test Story 1" }
                }
            };

            _mockNewsClient.Setup(x => x.GetNewStoryIds())
                          .ReturnsAsync(storyIds);

            _mockPagesProvider.Setup(x => x.GetStoryPages(storyIds, dto.PageNum, dto.PageSize, null))
                             .Returns(Task.FromResult<IEnumerable<IEnumerable<Story>>>(stories));

            // Act
            var result = await _controller.GetNewStories(dto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<IEnumerable<Story>>>());
            var returnedStories = (IEnumerable<IEnumerable<Story>>)okResult.Value;
            Assert.That(returnedStories, Is.EqualTo(stories));
        }

        [Test]
        public async Task GetNewStories_NewsClientReturnsNull_HandlesGracefully()
        {
            // Arrange
            var dto = new NewsRequestDTO
            {
                PageNum = 1,
                PageSize = 10,
                SearchTerm = "test"
            };

            _mockNewsClient.Setup(x => x.GetNewStoryIds())
                          .ReturnsAsync((int[]?)null);

            _mockPagesProvider.Setup(x => x.GetStoryPages(null, dto.PageNum, dto.PageSize, dto.SearchTerm))
                             .Returns(Task.FromResult<IEnumerable<IEnumerable<Story>>>(new List<IEnumerable<Story>>()));

            // Act
            var result = await _controller.GetNewStories(dto);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());

            _mockNewsClient.Verify(x => x.GetNewStoryIds(), Times.Once);
            _mockPagesProvider.Verify(x => x.GetStoryPages(null, dto.PageNum, dto.PageSize, dto.SearchTerm), Times.Once);
        }

        [Test]
        public async Task GetNewStories_NewsClientThrowsException_ExceptionPropagates()
        {
            // Arrange
            var dto = new NewsRequestDTO
            {
                PageNum = 1,
                PageSize = 10,
                SearchTerm = "test"
            };

            _mockNewsClient.Setup(x => x.GetNewStoryIds())
                          .ThrowsAsync(new Exception("News service unavailable"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _controller.GetNewStories(dto));
            Assert.That(ex.Message, Is.EqualTo("News service unavailable"));

            _mockNewsClient.Verify(x => x.GetNewStoryIds(), Times.Once);
            _mockPagesProvider.Verify(x => x.GetStoryPages(It.IsAny<int[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task GetNewStories_PagesProviderThrowsException_ExceptionPropagates()
        {
            // Arrange
            var dto = new NewsRequestDTO
            {
                PageNum = 1,
                PageSize = 10,
                SearchTerm = "test"
            };

            var storyIds = new[] { 1, 2, 3 };

            _mockNewsClient.Setup(x => x.GetNewStoryIds())
                          .ReturnsAsync(storyIds);

            _mockPagesProvider.Setup(x => x.GetStoryPages(storyIds, dto.PageNum, dto.PageSize, dto.SearchTerm))
                             .Throws(new Exception("Pages service unavailable"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _controller.GetNewStories(dto));
            Assert.That(ex.Message, Is.EqualTo("Pages service unavailable"));

            _mockNewsClient.Verify(x => x.GetNewStoryIds(), Times.Once);
            _mockPagesProvider.Verify(x => x.GetStoryPages(storyIds, dto.PageNum, dto.PageSize, dto.SearchTerm), Times.Once);
        }

        [Test]
        public async Task GetNewStories_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var dto = new NewsRequestDTO();
            _controller.ModelState.AddModelError("PageNum", "PageNum is required");

            // Act
            var result = await _controller.GetNewStories(dto);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.That(badRequestResult.Value, Is.InstanceOf<SerializableError>());

            // Verify that dependencies are not called when model state is invalid
            _mockNewsClient.Verify(x => x.GetNewStoryIds(), Times.Never);
            _mockPagesProvider.Verify(x => x.GetStoryPages(It.IsAny<int[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task GetNewStories_EmptyStoryIds_ReturnsNotFound()
        {
            // Arrange
            var dto = new NewsRequestDTO
            {
                PageNum = 1,
                PageSize = 10,
                SearchTerm = "test"
            };

            var emptyStoryIds = new int[0];
            var emptyStories = new List<IEnumerable<Story>>();

            _mockNewsClient.Setup(x => x.GetNewStoryIds())
                          .ReturnsAsync(emptyStoryIds);

            _mockPagesProvider.Setup(x => x.GetStoryPages(emptyStoryIds, dto.PageNum, dto.PageSize, dto.SearchTerm))
                             .Returns(Task.FromResult<IEnumerable<IEnumerable<Story>>>(emptyStories));

            // Act
            var result = await _controller.GetNewStories(dto);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        [TestCase(1, 5, "search")]
        [TestCase(2, 20, "")]
        [TestCase(10, 100, null)]
        public async Task GetNewStories_VariousValidInputs_CallsDependenciesWithCorrectParameters(int pageNum, int pageSize, string? searchTerm)
        {
            // Arrange
            var dto = new NewsRequestDTO
            {
                PageNum = pageNum,
                PageSize = pageSize,
                SearchTerm = searchTerm
            };

            var storyIds = new[] { 1, 2, 3 };
            var stories = new List<IEnumerable<Story>>
            {
                new List<Story> { new Story { Id = 1, Title = "Test" } }
            };

            _mockNewsClient.Setup(x => x.GetNewStoryIds())
                          .ReturnsAsync(storyIds);

            _mockPagesProvider.Setup(x => x.GetStoryPages(storyIds, pageNum, pageSize, searchTerm))
                             .Returns(Task.FromResult<IEnumerable<IEnumerable<Story>>>(stories));

            // Act
            var result = await _controller.GetNewStories(dto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            _mockPagesProvider.Verify(x => x.GetStoryPages(storyIds, pageNum, pageSize, searchTerm), Times.Once);
        }
    }
}