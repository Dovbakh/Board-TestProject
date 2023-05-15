using Board.Contracts.Contexts.AdvertFavorites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.AdvertFavorites.Services
{
    public interface IAdvertFavoriteService
    {
        Task AddIfNotExistsAsync(Guid advertId, CancellationToken cancellation);
        Task DeleteAsync(Guid advertId, CancellationToken cancellation);
        Task<IReadOnlyCollection<Guid>> GetAllForCurrentUserAsync(CancellationToken cancellation);
    }
}
