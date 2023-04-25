using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
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

        public async Task SetWithId(string key, TEntity entity)
        {
            var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(7));

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
            var cache = await _distributedCache.GetStringAsync(key);

            if (!string.IsNullOrEmpty(cache))
            {
                var cachedEntity = JsonConvert.DeserializeObject(cache, type);
                return cachedEntity;
            }

            return null;
        }

        public async Task SetWithId(string key, Type type, object entity)
        {
            var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(7));

            await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(entity, type, null), options);
        }
    }
}
