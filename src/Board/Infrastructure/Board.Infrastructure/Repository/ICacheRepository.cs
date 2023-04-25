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

        Task SetWithId(string key, Type type, object entity);
    }

    public interface ICacheRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetById(string key);

        Task SetWithId(string key, TEntity entity);
    }
}
