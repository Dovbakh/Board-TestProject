using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.AdvertViews.Services
{
    public interface IAdvertViewService
    {
        Task<int> GetCountAsync(Guid advertId, CancellationToken cancellation);
        Task<Guid> AddAsync(Guid advertId, CancellationToken cancellation);       
    }
}
