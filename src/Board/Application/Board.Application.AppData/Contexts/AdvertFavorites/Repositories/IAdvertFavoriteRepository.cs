using Board.Contracts.Contexts.AdvertFavorites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.AdvertFavorites.Repositories
{
    public interface IAdvertFavoriteRepository
    {
        Task<Guid> AddAsync(Guid advertId, Guid userId, CancellationToken cancellation);
        Task DeleteAsync(Guid advertFavoriteId, CancellationToken cancellation);
        Task<IReadOnlyCollection<AdvertFavoriteSummary>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellation);
    }
}
