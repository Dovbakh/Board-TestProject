using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.Repository
{
    /// <inheritdoc />
    public class CacheRepository<TEntity> : ICacheRepository<TEntity> where TEntity : class
    {
        private readonly IDistributedCache _distributedCache;

        public CacheRepository(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        /// <inheritdoc />
        public async Task<TEntity> GetById(string key, CancellationToken cancellation)
        {
            var cache = await _distributedCache.GetStringAsync(key, cancellation);

            if (!string.IsNullOrEmpty(cache))
            {
                var cachedEntity = JsonConvert.DeserializeObject<TEntity>(cache);
                return cachedEntity;
            }
            return null;
        }

        /// <inheritdoc />
        public async Task SetWithSlidingTime(string key, TEntity entity, TimeSpan slidingTime, CancellationToken cancellation)
        {
            var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(slidingTime);

            await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(entity), options, cancellation);
        }

        /// <inheritdoc />
        public async Task SetWithAbsoluteTime(string key, TEntity entity, TimeSpan absoluteTime, CancellationToken cancellation)
        {
            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(absoluteTime);

            await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(entity), options, cancellation);
        }

        /// <inheritdoc />
        public async Task DeleteAsync(string key, CancellationToken cancellation)
        {
            try
            {
                await _distributedCache.RemoveAsync(key, cancellation);
            }
            catch (RedisConnectionException ex)
            {

            }
        }
    }

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
        public async Task<object> GetById(string key, Type type, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение ресурса из кэша с ключом: {1}. Тип: {2}",
                nameof(DeleteAsync), key, nameof(Type));

            try
            {
                var cache = await _distributedCache.GetStringAsync(key, cancellation);

                if (string.IsNullOrEmpty(cache))
                {
                    return null;
                }

                var cachedEntity = JsonConvert.DeserializeObject(cache, type);
                return cachedEntity;

            }
            catch (RedisConnectionException ex)
            {
                _logger.LogWarning(ex, "{0} -> Не удалось подключение к серверу Redis.", nameof(GetById));
                return null;
            }
        }

        /// <inheritdoc />
        public async Task SetWithSlidingTime(string key, Type type, object entity, TimeSpan slidingTime, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Создание ресурса в кэше с ключом: {1}. Тип: {2}, данные: {3}, слайдовое время хранения: {4}",
                nameof(DeleteAsync), key, nameof(Type), JsonConvert.SerializeObject(entity), slidingTime);

            var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(slidingTime);

            try
            {
                await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(entity, type, null), options, cancellation);
            }
            catch (RedisConnectionException ex)
            {
                _logger.LogWarning(ex, "{0} -> Не удалось подключение к серверу Redis.", nameof(SetWithSlidingTime));
            }
        }

        /// <inheritdoc />
        public async Task SetWithAbsoluteTime(string key, Type type, object entity, TimeSpan absoluteTime, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Создание ресурса в кэше с ключом: {1}. Тип: {2}, данные: {3}, абсолютное время хранения: {4}",
                nameof(DeleteAsync), key, nameof(Type), JsonConvert.SerializeObject(entity), absoluteTime);

            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(absoluteTime);

            try
            {
                await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(entity, type, null), options, cancellation);
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
