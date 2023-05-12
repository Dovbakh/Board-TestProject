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
        private const string AdvertViewCountKey = "AdvertViewCountKey_";

        public AdvertViewRepository(IRepository<AdvertView> repository, IDistributedLockFactory distributedLockFactory, ILogger<AdvertViewRepository> logger)
        {
            _repository = repository;
            _distributedLockFactory = distributedLockFactory;
            _logger = logger;
        }

        public async Task<int> GetCountAsync(Guid advertId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение количества просмотров обьявления с ID: {1}",
                nameof(GetCountAsync), advertId);

            var viewCount = await _repository.GetAll()
                .Where(af => af.AdvertId == advertId)
                .CountAsync(cancellation);

            return viewCount;
        }

        public async Task<Guid> AddAsync(Guid advertId, Guid visitorId, bool isRegistered, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Создание записи о просмотре обьявления с ID: {1} посетителем с ID: {2}",
                nameof(AddAsync), advertId, visitorId);

            var existingAdvertView = await _repository.GetAll()
                .Where(av => av.AdvertId == advertId && av.VisitorId == visitorId)
                .FirstOrDefaultAsync(cancellation);
            if (existingAdvertView != null) 
            {
                return existingAdvertView.Id;
            }

            var advertView = new AdvertView 
            { 
                AdvertId = advertId, 
                VisitorId = visitorId, 
                CreatedAt = DateTime.UtcNow,
                isRegistered = isRegistered
            };
            await _repository.AddAsync(advertView, cancellation);

            return advertView.Id;
        }
    }
}

