using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Trainee.Api.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Trainee.Api.Services{
    public class RedisService : IRedisService
    {
        private readonly ILogger<RedisService> _logger;
        private readonly RedisSettings _settings;
        private readonly IDistributedCache _cache;
        private static readonly JsonSerializerOptions _jsonoptions = new JsonSerializerOptions()
        {
            
            PropertyNameCaseInsensitive = true
        };
        public RedisService(ILogger<RedisService> logger, IOptions<RedisSettings> options, IDistributedCache cache)
        {
            _cache = cache;
            _settings = options.Value;
            _logger = logger;
        }
        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                    throw new ArgumentException("Key Cannot be null");
                var cachedValue = await _cache.GetStringAsync(key);
                if(string.IsNullOrWhiteSpace(cachedValue))
                {
                    _logger.LogInformation("Cache Miss for key {k}", key);
                    return default;
                }
                _logger.LogInformation("Cache Hit for key {k}", key);
                return JsonSerializer.Deserialize<T>(cachedValue, _jsonoptions);
                
            }
            catch (Exception)
            {
                _logger.LogCritical("Redis Connection has failed..");
                return default;
                
            }
        }
        public async Task SetAsync<T>(T t, string key)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                    throw new ArgumentException("Key Cannot be null");
                var serialized_data = JsonSerializer.Serialize(t);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_settings.TTL),
                    SlidingExpiration = TimeSpan.FromMinutes(_settings.SlidingExpiration)
                };
                await _cache.SetStringAsync(key, serialized_data, options);
                _logger.LogInformation("Cache Set for key: {k}", key);   
            }
            catch (Exception)
            {
                _logger.LogCritical("Redis Connection has failed..");
                return ;
            }
            
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                    throw new ArgumentException("Key Cannot be null");
                await _cache.RemoveAsync(key);
                _logger.LogInformation("Cache Removed for key: {k}", key);
            }
            catch (Exception)
            {
                _logger.LogCritical("Redis Connection has failed..");
                return ;
            }
        }

    }
    
}
