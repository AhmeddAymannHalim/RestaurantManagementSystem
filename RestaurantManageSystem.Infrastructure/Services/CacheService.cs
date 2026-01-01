using Microsoft.Extensions.Caching.Memory;
using RestaurantManageSystem.Application.Interfaces;
using System.Text.Json;

namespace RestaurantManageSystem.Infrastructure.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<T> GetAsync<T>(string key) where T : class
        {
            return await Task.FromResult(_memoryCache.Get<T>(key));
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
        {
            var cacheOptions = new MemoryCacheEntryOptions();
            
            if (expiration.HasValue)
            {
                cacheOptions.AbsoluteExpirationRelativeToNow = expiration;
            }
            else
            {
                cacheOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
            }

            _memoryCache.Set(key, value, cacheOptions);
            await Task.CompletedTask;
        }

        public async Task RemoveAsync(string key)
        {
            _memoryCache.Remove(key);
            await Task.CompletedTask;
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            // Note: IMemoryCache doesn't support pattern-based removal
            // This would require storing keys separately or using a more advanced caching library
            await Task.CompletedTask;
        }
    }
}
