using Board.Contracts.Contexts.AdvertFavorites;
using Board.Contracts.Contexts.Adverts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.AdvertFavorites.Repositories
{
    public interface IAdvertFavoriteRepository
    {
        Task<IReadOnlyCollection<Guid>> GetIdsByUserIdAsync(Guid userId, CancellationToken cancellation);
        Task<Guid> AddIfNotExistsAsync(Guid advertId, Guid userId, CancellationToken cancellation);
        Task DeleteAsync(Guid advertId, Guid userId, CancellationToken cancellation);

        void AddToCookieIfNotExists(Guid advertId, CancellationToken cancellation);

        void DeleteFromCookie(Guid advertId, CancellationToken cancellation);

        List<Guid> GetIdsFromCookie(CancellationToken cancellation);


    }
}
