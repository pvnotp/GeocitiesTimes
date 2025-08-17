
using GeocitiesTimes.Server.Models;
using GeocitiesTimes.Server.Providers.Pages;
using GeocitiesTimes.Server.Providers.Stories;
using Moq;
using NUnit.Framework;

namespace GeocitiesTimes.Server.Tests.Providers.Pages
{
    [TestFixture]
    public class PagesProviderTests
    {
        private Mock<IStoriesProvider> _mockStoriesProvider;
        private PagesProvider _pagesProvider;

        [SetUp]
        public void SetUp()
        {
            _mockStoriesProvider = new Mock<IStoriesProvider>();
            _pagesProvider = new PagesProvider(_mockStoriesProvider.Object);
        }

        [Test]
        public async Task GetStoryPages_WithNoSearchTerm_ReturnsAllStoriesInPages()
        {
            var storyIds = new[] { 1, 2, 3, 4, 5 };
            var stories = new List<Story>
            {
                new Story { Id = 1, Title = "" },
                new Story { Id = 2, Title = "Turn around" },
                new Story { Id = 3, Title = "Every now and then I get a little bit lonely and you're never coming round." },
                new Story { Id = 4, Title = "Turn around" },
                new Story { Id = 5, Title = "Every now and then I get a little bit tired of listening to the sound of my tears." }
            };
            _mockStoriesProvider.Setup(x => x.GetStoryListFromCacheOrClient(It.IsAny<int[]>()))
                              .ReturnsAsync(stories);

            var result = await _pagesProvider.GetStoryPages(storyIds, pageNum: 1, pageSize: 2);

            _mockStoriesProvider.Verify(x => x.GetStoryListFromCacheOrClient(It.IsAny<int[]>()), Times.Once());
            
            var pages = result.ToList();
            Assert.That(pages, Has.Count.EqualTo(3));
            Assert.That(pages[0], Has.Length.EqualTo(2));
            Assert.That(pages[1], Has.Length.EqualTo(2));
            Assert.That(pages[2], Has.Length.EqualTo(1));
        }

        [Test]
        public async Task GetStoryPages_WithSearchTerm_ReturnsOnlyMatchingStories()
        {
            var storyIds = new[] { 1, 2, 3, 4, 5 };
            var stories = new List<Story>
            {
                new Story { Id = 1, Title = "" },
                new Story { Id = 2, Title = "Turn around" },
                new Story { Id = 3, Title = "Every now and then I get a little bit nervous that the best of all the years have gone by" },
                new Story { Id = 4, Title = "Turn around" },
                new Story { Id = 5, Title = "Every now and then I get a little bit terrified and then I see the look in your eyes." }
            };
            _mockStoriesProvider.Setup(x => x.GetStoryListFromCacheOrClient(It.IsAny<int[]>()))
                              .ReturnsAsync(stories);

            var result = await _pagesProvider.GetStoryPages(storyIds, pageNum: 1, pageSize: 2, searchTerm: "turn around");

            var pages = result.ToList();
            Assert.That(pages, Has.Count.EqualTo(1));
            Assert.That(pages[0], Has.Length.EqualTo(2));
            Assert.That(pages[0], Has.All.Matches<Story>(story => story.Title.Contains("turn around", StringComparison.OrdinalIgnoreCase)));
        }
    }
}