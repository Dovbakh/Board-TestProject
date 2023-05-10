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

            var advertView = await _repository.GetAll()
                .Where(af => af.AdvertId == advertId)
                .FirstOrDefaultAsync(cancellation);
            if (advertView == null)
            {
                throw new KeyNotFoundException($"Не найдено обьявление c ID: {advertId}");
            }

            return advertView.ViewCount;
        }

        public async Task<Guid> AddAsync(Guid advertId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Создание записи с количеством просмотров обьявления с ID: {1}",
                nameof(AddAsync), advertId);

            var existingAdvertView = await _repository.GetAll()
                .Where(av => av.AdvertId == advertId)
                .FirstOrDefaultAsync(cancellation);
            if (existingAdvertView != null) 
            {
                throw new ArgumentException($"Запись с количеством просмотра для обьявления с ID: {advertId} уже существует.");
            }
            var advertView = new AdvertView { AdvertId = advertId, ViewCount = 0 };
            await _repository.AddAsync(advertView, cancellation);

            return advertView.Id;
        }

        public async Task<int> UpdateCountAsync(Guid advertId, int count, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Увеличение количества просмотров обьявления с ID: {1} на {2}",
                nameof(UpdateCountAsync), advertId, count);

            var resource = AdvertViewCountKey + advertId;
            var expiry = TimeSpan.FromSeconds(30);
            var wait = TimeSpan.FromSeconds(10);
            var retry = TimeSpan.FromSeconds(1);
            await using (var redLock = await _distributedLockFactory.CreateLockAsync(resource, expiry, wait, retry, cancellation))
            {
                if (redLock.IsAcquired)
                {
                    var advertView = await _repository.GetAllFiltered(af => af.AdvertId == advertId).FirstOrDefaultAsync(cancellation);
                    if(advertView == null)
                    { 
                        throw new KeyNotFoundException($"Не найдено обьявление c ID: {advertId}");
                    }

                    advertView.ViewCount += count;
                    await _repository.UpdateAsync(advertView, cancellation);

                    return advertView.ViewCount;
                }
            }

            throw new Exception("Произошла ошибка при увеличении количества просмотров.");
        }


    }
}

