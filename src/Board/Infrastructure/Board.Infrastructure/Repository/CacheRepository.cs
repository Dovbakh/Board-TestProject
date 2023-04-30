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

        public async Task<TEntity> GetById(string key)
        {
            var cache = await _distributedCache.GetStringAsync(key);

            if (!string.IsNullOrEmpty(cache))
            {
                var cachedEntity = JsonConvert.DeserializeObject<TEntity>(cache);
                return cachedEntity;
            }
            return null;
        }

        public async Task SetWithSlidingTime(string key, TEntity entity, TimeSpan slidingTime)
        {
            var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(slidingTime);

            await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(entity), options);
        }

        public async Task SetWithAbsoluteTime(string key, TEntity entity, TimeSpan absoluteTime)
        {
            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(absoluteTime);

            await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(entity), options);
        }
    }


    public class CacheRepository : ICacheRepository
    {
        private readonly IDistributedCache _distributedCache;

        public CacheRepository(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<object> GetById(string key, Type type)
        {
            try
            {
                var cache = await _distributedCache.GetStringAsync(key);

                if (string.IsNullOrEmpty(cache))
                {
                    return null;
                }
               
                var cachedEntity = JsonConvert.DeserializeObject(cache, type);
                return cachedEntity;

            }
            catch(RedisConnectionException ex)
            {
                return null;
            }
        }

        public async Task SetWithSlidingTime(string key, Type type, object entity, TimeSpan slidingTime)
        {
            var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(slidingTime);

            try
            {
                await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(entity, type, null), options);
            }
            catch (RedisConnectionException ex)
            {

            }
        }

        public async Task SetWithAbsoluteTime(string key, Type type, object entity, TimeSpan absoluteTime)
        {
            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(absoluteTime);

            try
            {
                await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(entity, type, null), options);
            }
            catch (RedisConnectionException ex)
            {

            }
}
    }
}
