using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.AdvertViews.Services
{
    public interface IAdvertViewService
    {
        Task IncreaseCount(Guid advertId, CancellationToken cancellation);
        Task<int> GetCount(Guid advertId, CancellationToken cancellation);
    }
}
