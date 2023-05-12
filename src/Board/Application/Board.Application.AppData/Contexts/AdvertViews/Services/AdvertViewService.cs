using Board.Application.AppData.Contexts.Adverts.Repositories;
using Board.Application.AppData.Contexts.AdvertViews.Repositories;
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
    public class AdvertViewService : IAdvertViewService
    {
        private readonly IAdvertViewRepository _advertViewRepository;
        private readonly ILogger<AdvertViewService> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IAdvertRepository _advertRepository;
        private readonly IUserService _userService;
        private const string AdvertVisitorKey = "AdvertVisitorKey_";
        public AdvertViewService(IAdvertViewRepository advertViewRepository, ILogger<AdvertViewService> logger, IHttpContextAccessor contextAccessor,
            IAdvertRepository advertRepository, IUserService userService)
        {
            _advertViewRepository = advertViewRepository;
            _logger = logger;
            _contextAccessor = contextAccessor;
            _advertRepository = advertRepository;
            _userService = userService;
        }



        public Task<int> GetCountAsync(Guid advertId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение количества просмотров обьявления с ID: {1}",
                nameof(GetCountAsync), advertId);

            return _advertViewRepository.GetCountAsync(advertId, cancellation);
        }
        public async Task<Guid> AddAsync(Guid advertId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Создание записи о просмотре обьявления с ID: {1}",
                nameof(AddAsync), advertId);

            var isAdvertExists = await _advertRepository.IsExist(advertId, cancellation);
            if (!isAdvertExists) 
            {
                throw new KeyNotFoundException($"Не найдено обьявление с ID: {advertId}");
            }

            await _userService.isLogined(cancellation);

            var currentUserId = _userService.GetCurrentId(cancellation);
            if(currentUserId != Guid.Empty)
            {
                var adverViewId = await _advertViewRepository.AddAsync(advertId, currentUserId, true, cancellation);

                return adverViewId;
            }

            var advertVisitorId = _contextAccessor.HttpContext.Request.Cookies[AdvertVisitorKey];
            if(advertVisitorId == null)
            {
                advertVisitorId = Guid.NewGuid().ToString();
                _contextAccessor.HttpContext.Response.Cookies.Append(AdvertVisitorKey, advertVisitorId);
            }

            var advertViewId = await _advertViewRepository.AddAsync(advertId, Guid.Parse(advertVisitorId), false, cancellation);

            return advertViewId;
        }
    }
}
