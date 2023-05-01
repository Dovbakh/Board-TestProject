using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.Repository
{
    public class CacheRepository<TEntity> : ICacheRepository<TEntity> where TEntity : class
    {
        private readonly IDistributedCache _distributedCache;

        public CacheRepository(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

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

        public async Task SetWithSlidingTime(string key, TEntity entity, TimeSpan slidingTime, CancellationToken cancellation)
        {
            var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(slidingTime);

            await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(entity), options, cancellation);
        }

        public async Task SetWithAbsoluteTime(string key, TEntity entity, TimeSpan absoluteTime, CancellationToken cancellation)
        {
            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(absoluteTime);

            await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(entity), options, cancellation);
        }

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


    public class CacheRepository : ICacheRepository
    {
        private readonly IDistributedCache _distributedCache;

        public CacheRepository(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<object> GetById(string key, Type type, CancellationToken cancellation)
        {
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
                return null;
            }
        }

        public async Task SetWithSlidingTime(string key, Type type, object entity, TimeSpan slidingTime, CancellationToken cancellation)
        {
            var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(slidingTime);

            try
            {
                await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(entity, type, null), options, cancellation);
            }
            catch (RedisConnectionException ex)
            {

            }
        }

        public async Task SetWithAbsoluteTime(string key, Type type, object entity, TimeSpan absoluteTime, CancellationToken cancellation)
        {
            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(absoluteTime);

            try
            {
                await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(entity, type, null), options, cancellation);
            }
            catch (RedisConnectionException ex)
            {

            }
        }
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
}
