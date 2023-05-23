using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Repository
{
    /// <inheritdoc />
    public class CacheRepository : ICacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<CacheRepository> _logger;

        public CacheRepository(IDistributedCache distributedCache, ILogger<CacheRepository> logger)
        {
            _distributedCache = distributedCache;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<T> GetById<T>(string key, CancellationToken cancellation) where T: class
        {
            _logger.LogInformation("{0} -> Получение ресурса из кэша с ключом: {1}. Тип: {2}",
                nameof(DeleteAsync), key, typeof(T));

            try
            {
                var cache = await _distributedCache.GetStringAsync(key, cancellation);

                if (string.IsNullOrEmpty(cache))
                {
                    return null;
                }

                var cachedEntity = JsonConvert.DeserializeObject<T>(cache);
                return cachedEntity;

            }
            catch (RedisConnectionException ex)
            {
                _logger.LogWarning(ex, "{0} -> Не удалось подключение к серверу Redis.", nameof(GetById));
                return null;
            }
        }

        /// <inheritdoc />
        public async Task SetWithSlidingTime(string key, object entity, TimeSpan slidingTime, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Создание ресурса в кэше с ключом: {1}. Тип: {2}, данные: {3}, слайдовое время хранения: {4}",
                nameof(DeleteAsync), key, typeof(object), JsonConvert.SerializeObject(entity), slidingTime);

            var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(slidingTime);

            try
            {
                await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(entity), options, cancellation);
            }
            catch (RedisConnectionException ex)
            {
                _logger.LogWarning(ex, "{0} -> Не удалось подключение к серверу Redis.", nameof(SetWithSlidingTime));
            }
        }

        /// <inheritdoc />
        public async Task SetWithAbsoluteTime(string key, object entity, TimeSpan absoluteTime, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Создание ресурса в кэше с ключом: {1}. Тип: {2}, данные: {3}, абсолютное время хранения: {4}",
                nameof(DeleteAsync), key, typeof(object), JsonConvert.SerializeObject(entity), absoluteTime);

            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(absoluteTime);

            try
            {
                await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(entity), options, cancellation);
            }
            catch (RedisConnectionException ex)
            {
                _logger.LogWarning(ex, "{0} -> Не удалось подключение к серверу Redis.", nameof(SetWithAbsoluteTime));
            }
        }

        /// <inheritdoc />
        public async Task DeleteAsync(string key, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Инвалидация ресурса из кэша с ключом: {1}",
                nameof(DeleteAsync), key);

            try
            {
                await _distributedCache.RemoveAsync(key, cancellation);
            }
            catch (RedisConnectionException ex)
            {
                _logger.LogWarning(ex, "{0} -> Не удалось подключение к серверу Redis.", nameof(DeleteAsync));
            }
        }
    }


   
}
