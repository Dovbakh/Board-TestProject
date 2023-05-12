using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.AdvertViews.Repositories
{
    public interface IAdvertViewRepository
    {
        Task<int> GetCountAsync(Guid advertId, CancellationToken cancellation);
        Task<Guid> AddAsync(Guid advertId, Guid visitorId, bool isRegistered, CancellationToken cancellation);
     
    }
}
