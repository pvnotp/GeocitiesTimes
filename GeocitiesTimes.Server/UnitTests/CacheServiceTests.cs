using GeocitiesTimes.Server.Models;
using GeocitiesTimes.Server.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace GeocitiesTimes.Server.Tests.Services
{
    [TestFixture]
    public class CacheServiceTests
    {
        private IMemoryCache _memoryCache;
        private CacheService _cacheService;

        [SetUp]
        public void SetUp()
        {
            // Create a real MemoryCache instance for testing
            var options = Options.Create(new MemoryCacheOptions());
            _memoryCache = new MemoryCache(options);
            _cacheService = new CacheService(_memoryCache);
        }

        [Test]
        public void TryGetStoryFromCache_WhenStoryExists_ReturnsTrue()
        {
            var story = new Story { Id = 1, Title = "Test Story" };
            _memoryCache.Set(1, story);

            var result = _cacheService.TryGetStoryFromCache(1, out Story retrievedStory);

            Assert.That(result, Is.True);
            Assert.That(retrievedStory, Is.Not.Null);
            Assert.That(retrievedStory.Id, Is.EqualTo(1));
            Assert.That(retrievedStory.Title, Is.EqualTo("Test Story"));
        }

        [Test]
        public void TryGetStoryFromCache_WhenStoryDoesNotExist_ReturnsFalse()
        {
            var result = _cacheService.TryGetStoryFromCache(999, out Story retrievedStory);

            Assert.That(result, Is.False);
            Assert.That(retrievedStory, Is.Null);
        }

        [Test]
        public void AddStoryToCache_WhenStoryIsValid_AddsToCache()
        {
            var story = new Story { Id = 2, Title = "Another Test Story" };

            _cacheService.AddStoryToCache(story);

            var cacheResult = _memoryCache.TryGetValue(2, out Story cachedStory);
            Assert.That(cacheResult, Is.True);
            Assert.That(cachedStory, Is.Not.Null);
            Assert.That(cachedStory.Id, Is.EqualTo(2));
            Assert.That(cachedStory.Title, Is.EqualTo("Another Test Story"));
        }

    }
}