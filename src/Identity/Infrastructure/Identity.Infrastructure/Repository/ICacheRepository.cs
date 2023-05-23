using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Repository
{
    public interface ICacheRepository
    {
        Task<T> GetById<T>(string key, CancellationToken cancellation) where T : class;
        Task SetWithSlidingTime(string key, object entity, TimeSpan slidingTime, CancellationToken cancellation);
        Task SetWithAbsoluteTime(string key, object entity, TimeSpan absoluteTime, CancellationToken cancellation);
        Task DeleteAsync(string key, CancellationToken cancellation);
    }

}
