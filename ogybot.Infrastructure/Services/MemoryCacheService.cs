using Microsoft.Extensions.Caching.Memory;
using ogybot.Domain.Entities.Configurations;
using ogybot.Domain.Services;

namespace ogybot.Data.Services;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    public MemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public T? QueryFor<T>(object key)
    {
        return _memoryCache.Get<T>(key);
    }

    public void InsertInto<T>(object key, T entity)
    {
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromHours(6))
            .SetSlidingExpiration(TimeSpan.FromMinutes(30));

        _memoryCache.Set(key, entity, cacheOptions);
    }
}