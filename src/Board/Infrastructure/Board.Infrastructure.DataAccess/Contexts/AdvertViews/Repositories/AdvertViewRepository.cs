using AutoMapper;
using Board.Application.AppData.Contexts.AdvertViews.Repositories;
using Board.Contracts.Contexts.AdvertImages;
using Board.Domain;
using Board.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RedLockNet;
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
        private readonly IDistributedLockFactory _distributedLockFactory;
        private readonly ILogger<AdvertViewRepository> _logger;

        public AdvertViewRepository(IRepository<AdvertView> repository, IDistributedLockFactory distributedLockFactory, ILogger<AdvertViewRepository> logger)
        {
            _repository = repository;
            _distributedLockFactory = distributedLockFactory;
            _logger = logger;
        }

        public async Task<int> GetCountAsync(Guid advertId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение количества просмотров обьявления с ID: {2}",
                nameof(AdvertViewRepository), nameof(GetCountAsync), advertId);

            var viewCount = await _repository.GetAll()
                .Where(af => af.AdvertId == advertId)
                .CountAsync(cancellation);

            return viewCount;
        }

        public async Task<Guid> AddIfNotExistsAsync(Guid advertId, Guid visitorId, bool isRegistered, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Создание записи о просмотре обьявления с ID: {2} посетителем с ID: {3}",
                 nameof(AdvertViewRepository), nameof(AddIfNotExistsAsync), advertId, visitorId);

            var advertView = await _repository.GetAll()
                .Where(av => av.AdvertId == advertId && av.VisitorId == visitorId)
                .FirstOrDefaultAsync(cancellation);
            if (advertView != null) 
            {
                return advertView.Id;
            }

            var newAdvertView = new AdvertView 
            { 
                AdvertId = advertId, 
                VisitorId = visitorId, 
                CreatedAt = DateTime.UtcNow,
                IsRegistered = isRegistered
            };
            await _repository.AddAsync(newAdvertView, cancellation);

            return newAdvertView.Id;
        }
    }
}

