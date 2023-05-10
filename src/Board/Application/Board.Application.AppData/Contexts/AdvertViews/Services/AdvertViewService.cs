using Board.Application.AppData.Contexts.AdvertViews.Repositories;
using Board.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Logging;
using RedLockNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.AdvertViews.Services
{
    public class AdvertViewService : IAdvertViewService
    {
        private readonly IAdvertViewRepository _advertViewRepository;
        private readonly ILogger<AdvertViewService> _logger;
        public AdvertViewService(IAdvertViewRepository advertViewRepository, ILogger<AdvertViewService> logger)
        {
            _advertViewRepository = advertViewRepository;
            _logger = logger;
        }



        public Task<int> GetCountAsync(Guid advertId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение количества просмотров обьявления с ID: {1}",
                nameof(GetCountAsync), advertId);

            return _advertViewRepository.GetCountAsync(advertId, cancellation);
        }
        public async Task<Guid> AddAsync(Guid advertId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Создание записи с количеством просмотров обьявления с ID: {1}",
                nameof(AddAsync), advertId);

            var advertViewId = await _advertViewRepository.AddAsync(advertId, cancellation);

            return advertViewId;
        }

        public Task<int> UpdateCountAsync(Guid advertId, int count, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Увеличение количества просмотров обьявления с ID: {1} на {2}",
                nameof(UpdateCountAsync), advertId, count);

            return _advertViewRepository.UpdateCountAsync(advertId, count, cancellation);
        }

    }
}
