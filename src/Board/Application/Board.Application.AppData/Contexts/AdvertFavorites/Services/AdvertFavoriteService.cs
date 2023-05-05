using Board.Contracts.Contexts.Adverts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.AdvertFavorites.Services
{
    public class AdvertFavoriteService : IAdvertFavoriteService
    {
        public Task<Guid> AddAsync(Guid advertId, Guid userId, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid advertFavoriteId, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<AdvertSummary>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }
    }
}
