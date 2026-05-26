using System.Text.Json;
using eTerminiAPI.Application.Interfaces.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace eTerminiAPI.Infrastructure.Caching;

public class RedisCacheService : ICacheService
{
    private static readonly TimeSpan DefaultTtl = TimeSpan.FromMinutes(5);

    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var raw = await _cache.GetStringAsync(key, cancellationToken);
            if (string.IsNullOrEmpty(raw))
                return default;

            return JsonSerializer.Deserialize<T>(raw);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis GET dështoi për çelësin {Key}. Po anashkalohet cache-i.", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl ?? DefaultTtl
            };

            var payload = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, payload, options, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis SET dështoi për çelësin {Key}. Po anashkalohet cache-i.", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.RemoveAsync(key, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis REMOVE dështoi për çelësin {Key}.", key);
        }
    }
}
