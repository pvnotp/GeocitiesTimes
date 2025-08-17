
using GeocitiesTimes.Server.Clients;
using GeocitiesTimes.Server.Models;
using GeocitiesTimes.Server.Providers.Stories;
using GeocitiesTimes.Server.Services;
using Moq;
using NUnit.Framework;

namespace GeocitiesTimes.Server.Tests.Providers.Stories
{
    [TestFixture]
    public class StoriesProviderTests
    {
        private Mock<INewsClient> _mockNewsClient;
        private Mock<ICacheService> _mockCacheService;
        private StoriesProvider _storiesProvider;

        [SetUp]
        public void SetUp()
        {
            _mockNewsClient = new Mock<INewsClient>();
            _mockCacheService = new Mock<ICacheService>();
            _storiesProvider = new StoriesProvider(_mockNewsClient.Object, _mockCacheService.Object);
        }

        [Test]
        public async Task GetStoryFromCacheOrClient_WhenStoryInCache_ReturnsStoryFromCache()
        {
            var storyId = 123;
            var cachedStory = new Story { Id = storyId, Title = "Cached Story" };

            _mockCacheService.Setup(x => x.TryGetStoryFromCache(storyId, out cachedStory))
                           .Returns(true);

            var result = await _storiesProvider.GetStoryFromCacheOrClient(storyId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(storyId));
            Assert.That(result.Title, Is.EqualTo("Cached Story"));

            _mockCacheService.Verify(x => x.TryGetStoryFromCache(storyId, out cachedStory), Times.Once);
            _mockNewsClient.Verify(x => x.GetStory(It.IsAny<int>()), Times.Never);
            _mockCacheService.Verify(x => x.AddStoryToCache(It.IsAny<Story>()), Times.Never);
        }

        [Test]
        public async Task GetStoryFromCacheOrClient_WhenStoryNotInCacheButFoundByClient_ReturnsStoryFromClient()
        {
            var storyId = 123;
            var clientStory = new Story { Id = storyId, Title = "Client Story" };
            Story cachedStory = null;

            _mockCacheService.Setup(x => x.TryGetStoryFromCache(storyId, out cachedStory))
                           .Returns(false);
            _mockNewsClient.Setup(x => x.GetStory(storyId))
                          .ReturnsAsync(clientStory);

            var result = await _storiesProvider.GetStoryFromCacheOrClient(storyId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(storyId));
            Assert.That(result.Title, Is.EqualTo("Client Story"));

            _mockCacheService.Verify(x => x.TryGetStoryFromCache(storyId, out cachedStory), Times.Once);
            _mockNewsClient.Verify(x => x.GetStory(storyId), Times.Once);
            _mockCacheService.Verify(x => x.AddStoryToCache(clientStory), Times.Once);
        }

        [Test]
        public async Task GetStoryFromCacheOrClient_WhenStoryNotFoundInCacheOrClient_ReturnsNull()
        {
            var storyId = 123;
            Story cachedStory = null;

            _mockCacheService.Setup(x => x.TryGetStoryFromCache(storyId, out cachedStory))
                           .Returns(false);
            _mockNewsClient.Setup(x => x.GetStory(storyId))
                          .ReturnsAsync((Story)null);

            var result = await _storiesProvider.GetStoryFromCacheOrClient(storyId);

            Assert.That(result, Is.Null);

            _mockCacheService.Verify(x => x.TryGetStoryFromCache(storyId, out cachedStory), Times.Once);
            _mockNewsClient.Verify(x => x.GetStory(storyId), Times.Once);
            _mockCacheService.Verify(x => x.AddStoryToCache(It.IsAny<Story>()), Times.Never);
        }

        [Test]
        public async Task GetStoryListFromCacheOrClient_WhenStoryIdsIsEmpty_ReturnsEmptyList()
        {
            var result = await _storiesProvider.GetStoryListFromCacheOrClient(new int[0]);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);

            _mockCacheService.VerifyNoOtherCalls();
            _mockNewsClient.VerifyNoOtherCalls();
        }


        [Test]
        public async Task GetStoryListFromCacheOrClient_WhenSomeStoriesAreNull_FiltersOutNullStories()
        {
            var storyIds = new[] { 1, 2, 3 };
            var story1 = new Story { Id = 1, Title = "Story 1" };
            var story3 = new Story { Id = 3, Title = "Story 3" };

            Story cachedStory = null;
            _mockCacheService.Setup(x => x.TryGetStoryFromCache(It.IsAny<int>(), out cachedStory))
                           .Returns(false);

            _mockNewsClient.Setup(x => x.GetStory(1)).ReturnsAsync(story1);
            _mockNewsClient.Setup(x => x.GetStory(2)).ReturnsAsync((Story)null);
            _mockNewsClient.Setup(x => x.GetStory(3)).ReturnsAsync(story3);

            var result = await _storiesProvider.GetStoryListFromCacheOrClient(storyIds);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.Any(s => s.Id == 1), Is.True);
            Assert.That(result.Any(s => s.Id == 2), Is.False);
            Assert.That(result.Any(s => s.Id == 3), Is.True);
   

            _mockCacheService.Verify(x => x.AddStoryToCache(story1), Times.Once);
            _mockCacheService.Verify(x => x.AddStoryToCache(story3), Times.Once);
            _mockCacheService.Verify(x => x.AddStoryToCache(It.Is<Story>(s => s.Id == 2)), Times.Never);
        }

        [Test]
        public async Task GetStoryListFromCacheOrClient_WhenExceptionOccurs_ContinuesProcessingOtherStories()
        {
            var storyIds = new[] { 1, 2, 3 };
            var story1 = new Story { Id = 1, Title = "Story 1" };
            var story3 = new Story { Id = 3, Title = "Story 3" };

            Story cachedStory = null;
            _mockCacheService.Setup(x => x.TryGetStoryFromCache(It.IsAny<int>(), out cachedStory))
                           .Returns(false);

            _mockNewsClient.Setup(x => x.GetStory(1)).ReturnsAsync(story1);
            _mockNewsClient.Setup(x => x.GetStory(2)).ThrowsAsync(new Exception("Oops"));
            _mockNewsClient.Setup(x => x.GetStory(3)).ReturnsAsync(story3);

            var result = await _storiesProvider.GetStoryListFromCacheOrClient(storyIds);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.Any(s => s.Id == 1), Is.True);
            Assert.That(result.Any(s => s.Id == 3), Is.True);

            _mockCacheService.Verify(x => x.AddStoryToCache(story1), Times.Once);
            _mockCacheService.Verify(x => x.AddStoryToCache(story3), Times.Once);
        }
    }
}