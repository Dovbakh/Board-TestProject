using Board.Contracts.Contexts.Adverts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.AdvertFavorites.Services
{
    public interface IAdvertFavoriteService
    {
        Task<Guid> AddAsync(Guid advertId, Guid userId, CancellationToken cancellation);
        Task DeleteAsync(Guid advertFavoriteId, CancellationToken cancellation);
        Task<IReadOnlyCollection<AdvertSummary>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellation);
    }
}
