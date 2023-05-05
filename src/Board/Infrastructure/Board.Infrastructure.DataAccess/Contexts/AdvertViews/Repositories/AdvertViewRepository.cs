using AutoMapper;
using Board.Application.AppData.Contexts.AdvertViews.Repositories;
using Board.Contracts.Contexts.AdvertImages;
using Board.Domain;
using Board.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.DataAccess.Contexts.AdvertViews.Repositories
{
    public class AdvertViewRepository : IAdvertViewRepository
    {
        private readonly IRepository<AdvertView> _repository;

        public AdvertViewRepository(IRepository<AdvertView> repository)
        {
            _repository = repository;
        }

        public Task<int> GetCount(Guid advertId, CancellationToken cancellation)
        {
            return _repository.GetAllFiltered(af => af.AdvertId == advertId).CountAsync(cancellation);
        }

        public async Task IncreaseCount(Guid advertId, CancellationToken cancellation)
        {
            var advertFavorite = await _repository.GetAllFiltered(af => af.AdvertId == advertId).FirstOrDefaultAsync(cancellation);
            advertFavorite.ViewCount += 1;

            await _repository.UpdateAsync(advertFavorite, cancellation);
        }
    }
}

