using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.AdvertViews.Repositories
{
    public interface IAdvertViewRepository
    {
        Task IncreaseCount(Guid advertId, CancellationToken cancellation);
        Task<int> GetCount(Guid advertId, CancellationToken cancellation);
    }
}
