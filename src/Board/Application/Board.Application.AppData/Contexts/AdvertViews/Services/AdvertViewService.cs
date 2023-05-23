using Board.Application.AppData.Contexts.Adverts.Repositories;
using Board.Application.AppData.Contexts.AdvertViews.Repositories;
using Board.Application.AppData.Contexts.Categories.Services;
using Board.Application.AppData.Contexts.Users.Services;
using Board.Infrastructure.Repository;
using Microsoft.AspNetCore.Http;
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
    /// <inheritdoc />
    public class AdvertViewService : IAdvertViewService
    {
        private readonly IAdvertViewRepository _advertViewRepository;
        private readonly ILogger<AdvertViewService> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IAdvertRepository _advertRepository;
        private readonly IUserService _userService;

        /// <inheritdoc />
        public AdvertViewService(IAdvertViewRepository advertViewRepository, ILogger<AdvertViewService> logger, IHttpContextAccessor contextAccessor,
            IAdvertRepository advertRepository, IUserService userService)
        {
            _advertViewRepository = advertViewRepository;
            _logger = logger;
            _contextAccessor = contextAccessor;
            _advertRepository = advertRepository;
            _userService = userService;
        }

        /// <inheritdoc />
        public Task<int> GetCountAsync(Guid advertId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение количества просмотров обьявления с ID: {2}",
                nameof(AdvertViewService), nameof(GetCountAsync), advertId);

            return _advertViewRepository.GetCountAsync(advertId, cancellation);
        }

        /// <inheritdoc />
        public async Task<Guid> AddIfNotExistsAsync(Guid advertId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Создание записи о просмотре обьявления с ID: {2}",
                nameof(AdvertViewService), nameof(AddIfNotExistsAsync), advertId);

            var isAdvertExists = await _advertRepository.IsExists(advertId, cancellation);
            if (!isAdvertExists) 
            {
                throw new KeyNotFoundException($"Не найдено обьявление с ID: {advertId}");
            }

            var isUserLogined = await _userService.IsLoginedAsync(cancellation);
            if (!isUserLogined)
            {
                var anonymousId = _userService.GetAnonymousId(cancellation);
                return await _advertViewRepository.AddIfNotExistsAsync(advertId, anonymousId, false, cancellation);
            }

            var currentUserId = _userService.GetCurrentId(cancellation).Value;
            return await _advertViewRepository.AddIfNotExistsAsync(advertId, currentUserId, true, cancellation);           
        }
    }
}
