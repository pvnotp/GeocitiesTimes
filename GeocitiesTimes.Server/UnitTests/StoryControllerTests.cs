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
        private NewsRequestDTO _defaultDTO;

        [SetUp]
        public void SetUp()
        {
            _mockPagesProvider = new Mock<IPagesProvider>();
            _mockNewsClient = new Mock<INewsClient>();
            _controller = new StoryController(_mockPagesProvider.Object, _mockNewsClient.Object);

            _defaultDTO = new NewsRequestDTO
            {
                PageNum = 1,
                PageSize = 10,
                SearchTerm = "test"
            };
        }

        [Test]
        public async Task GetNewStories_ValidRequest_ReturnsOkWithStories()
        {
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

            _mockPagesProvider.Setup(x => x.GetStoryPages(storyIds, _defaultDTO.PageNum, _defaultDTO.PageSize, _defaultDTO.SearchTerm))
                             .Returns(Task.FromResult<IEnumerable<IEnumerable<Story>>>(stories));

            var result = await _controller.GetNewStories(_defaultDTO);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<IEnumerable<Story>>>());
            var returnedStories = (IEnumerable<IEnumerable<Story>>)okResult.Value;
            Assert.That(returnedStories, Is.EqualTo(stories));

            _mockNewsClient.Verify(x => x.GetNewStoryIds(), Times.Once);
            _mockPagesProvider.Verify(x => x.GetStoryPages(storyIds, _defaultDTO.PageNum, _defaultDTO.PageSize, _defaultDTO.SearchTerm), Times.Once);
        }

        [Test]
        public async Task GetNewStories_NoStoriesFound_ReturnsEmptyList()
        {
            var dto = new NewsRequestDTO
            {
                PageNum = 1,
                PageSize = 10,
                SearchTerm = "abracadabra"
            };

            var storyIds = new[] { 1, 2, 3 };
            var emptyStories = new List<IEnumerable<Story>>();

            _mockNewsClient.Setup(x => x.GetNewStoryIds()).ReturnsAsync(storyIds);

            _mockPagesProvider.Setup(x => x.GetStoryPages(storyIds, dto.PageNum, dto.PageSize, dto.SearchTerm))
                             .Returns(Task.FromResult<IEnumerable<IEnumerable<Story>>>(emptyStories));

            var result = await _controller.GetNewStories(dto);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<IEnumerable<Story>>>());
            Assert.That(okResult.Value, Is.Empty);

            _mockNewsClient.Verify(x => x.GetNewStoryIds(), Times.Once);
            _mockPagesProvider.Verify(x => x.GetStoryPages(storyIds, dto.PageNum, dto.PageSize, dto.SearchTerm), Times.Once);
        }

        [Test]
        public async Task GetNewStories_WithNullSearchTerm_ReturnsOkWithStories()
        {
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

            _mockNewsClient.Setup(x => x.GetNewStoryIds()).ReturnsAsync(storyIds);

            _mockPagesProvider.Setup(x => x.GetStoryPages(storyIds, dto.PageNum, dto.PageSize, null))
                             .Returns(Task.FromResult<IEnumerable<IEnumerable<Story>>>(stories));

            var result = await _controller.GetNewStories(dto);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<IEnumerable<Story>>>());
            var returnedStories = (IEnumerable<IEnumerable<Story>>)okResult.Value;
            Assert.That(returnedStories, Is.EqualTo(stories));
        }

        [Test]
        public async Task GetNewStories_NewsClientReturnsNull_ReturnsNotFound()
        {
            _mockNewsClient.Setup(x => x.GetNewStoryIds())
                          .ReturnsAsync((int[]?)null);

            _mockPagesProvider.Setup(x => x.GetStoryPages(null, _defaultDTO.PageNum, _defaultDTO.PageSize, _defaultDTO.SearchTerm))
                             .Returns(Task.FromResult<IEnumerable<IEnumerable<Story>>>(new List<IEnumerable<Story>>()));

            var result = await _controller.GetNewStories(_defaultDTO);

            _mockNewsClient.Verify(x => x.GetNewStoryIds(), Times.Once);
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task GetNewStories_InvalidModelState_ReturnsBadRequest()
        {
            var dto = new NewsRequestDTO();
            _controller.ModelState.AddModelError("PageNum", "PageNum is required");

            var result = await _controller.GetNewStories(dto);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.That(badRequestResult.Value, Is.InstanceOf<SerializableError>());

            // Verify that dependencies are not called when model state is invalid
            _mockNewsClient.Verify(x => x.GetNewStoryIds(), Times.Never);
            _mockPagesProvider.Verify(x => x.GetStoryPages(It.IsAny<int[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }
    }
}