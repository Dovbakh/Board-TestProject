using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.Repository
{
    public interface ICacheRepository
    {
        Task<object> GetById(string key, Type type, CancellationToken cancellation);

        Task SetWithSlidingTime(string key, Type type, object entity, TimeSpan slidingTime, CancellationToken cancellation);
        Task SetWithAbsoluteTime(string key, Type type, object entity, TimeSpan absoluteTime, CancellationToken cancellation);
        Task DeleteAsync(string key, CancellationToken cancellation);
    }

    public interface ICacheRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetById(string key, CancellationToken cancellation);

        Task SetWithSlidingTime(string key, TEntity entity, TimeSpan slidingTime, CancellationToken cancellation);

        Task SetWithAbsoluteTime(string key, TEntity entity, TimeSpan absoluteTime , CancellationToken cancellation);

        Task DeleteAsync(string key, CancellationToken cancellation);
    }
}
