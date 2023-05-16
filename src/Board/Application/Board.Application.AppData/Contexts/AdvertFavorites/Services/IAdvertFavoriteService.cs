using Board.Contracts.Contexts.AdvertFavorites;
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
        Task<IReadOnlyCollection<AdvertSummary>> GetAdvertsForCurrentUserAsync(int? offset, int? limit, CancellationToken cancellation);
        Task AddIfNotExistsAsync(Guid advertId, CancellationToken cancellation);
        Task DeleteAsync(Guid advertId, CancellationToken cancellation);
        Task<IReadOnlyCollection<Guid>> GetIdsForCurrentUserAsync(CancellationToken cancellation);

        
    }
}
