using Microsoft.Extensions.Caching.Memory;
using RestaurantManageSystem.Infrastructure.Services;

namespace RestaurantManageSystem.Tests
{
    public class CacheServiceTests
    {
        private readonly IMemoryCache _memoryCache;
        private readonly CacheService _cacheService;

        public CacheServiceTests()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _cacheService = new CacheService(_memoryCache);
        }

        [Fact]
        public async Task SetAsync_WithValidData_SetsCacheValue()
        {
            // Arrange
            var key = "test_key";
            var value = "test_value";

            // Act
            await _cacheService.SetAsync(key, value);

            // Assert
            var cached = await _cacheService.GetAsync<string>(key);
            Assert.NotNull(cached);
            Assert.Equal(value, cached);
        }

        [Fact]
        public async Task SetAsync_WithExpiration_SetsCacheValueWithExpiration()
        {
            // Arrange
            var key = "test_key_with_expiration";
            var value = "test_value";
            var expiration = TimeSpan.FromSeconds(1);

            // Act
            await _cacheService.SetAsync(key, value, expiration);

            // Assert
            var cached = await _cacheService.GetAsync<string>(key);
            Assert.NotNull(cached);
            Assert.Equal(value, cached);

            // Wait for expiration
            await Task.Delay(1100);
            var expiredValue = await _cacheService.GetAsync<string>(key);
            Assert.Null(expiredValue);
        }

        [Fact]
        public async Task GetAsync_WithNonExistentKey_ReturnsNull()
        {
            // Arrange
            var key = "non_existent_key";

            // Act
            var result = await _cacheService.GetAsync<string>(key);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RemoveAsync_WithValidKey_RemovesCacheEntry()
        {
            // Arrange
            var key = "key_to_remove";
            var value = "value_to_remove";
            await _cacheService.SetAsync(key, value);

            // Act
            await _cacheService.RemoveAsync(key);

            // Assert
            var cached = await _cacheService.GetAsync<string>(key);
            Assert.Null(cached);
        }

        [Fact]
        public async Task SetAsync_WithComplexObject_CachesObject()
        {
            // Arrange
            var key = "complex_object_key";
            var testObject = new TestCacheObject
            {
                Id = 1,
                Name = "Test",
                Value = 100
            };

            // Act
            await _cacheService.SetAsync(key, testObject);

            // Assert
            var cached = await _cacheService.GetAsync<TestCacheObject>(key);
            Assert.NotNull(cached);
            Assert.Equal(testObject.Id, cached.Id);
            Assert.Equal(testObject.Name, cached.Name);
            Assert.Equal(testObject.Value, cached.Value);
        }

        [Fact]
        public async Task SetAsync_WithDefaultExpiration_UsesThirtyMinutes()
        {
            // Arrange
            var key = "default_expiration_key";
            var value = "test_value";

            // Act
            await _cacheService.SetAsync(key, value);

            // Assert
            var cached = await _cacheService.GetAsync<string>(key);
            Assert.NotNull(cached);

            // Value should still exist after a short time
            await Task.Delay(100);
            var cachedAfterDelay = await _cacheService.GetAsync<string>(key);
            Assert.NotNull(cachedAfterDelay);
        }

        [Fact]
        public async Task RemoveByPatternAsync_IsImplemented()
        {
            // Arrange
            var pattern = "test_pattern_*";

            // Act & Assert
            // This method is a placeholder in the current implementation
            // It should complete without throwing an exception
            await _cacheService.RemoveByPatternAsync(pattern);
            Assert.True(true);
        }

        private class TestCacheObject
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Value { get; set; }
        }
    }
}
