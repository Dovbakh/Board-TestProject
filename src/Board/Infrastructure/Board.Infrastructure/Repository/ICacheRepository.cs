using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.Repository
{
    public interface ICacheRepository
    {
        Task<object> GetById(string key, Type type);

        Task SetWithSlidingTime(string key, Type type, object entity, TimeSpan slidingTime);
        Task SetWithAbsoluteTime(string key, Type type, object entity, TimeSpan absoluteTime);
    }

    public interface ICacheRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetById(string key);

        Task SetWithSlidingTime(string key, TEntity entity, TimeSpan slidingTime);

        Task SetWithAbsoluteTime(string key, TEntity entity, TimeSpan absoluteTime);
    }
}
