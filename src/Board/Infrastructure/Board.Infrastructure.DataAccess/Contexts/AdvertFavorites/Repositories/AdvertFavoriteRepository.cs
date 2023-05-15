using AutoMapper;
using AutoMapper.QueryableExtensions;
using Board.Application.AppData.Contexts.AdvertFavorites.Repositories;
using Board.Application.AppData.Contexts.AdvertFavorites.Services;
using Board.Contracts.Contexts.AdvertFavorites;
using Board.Contracts.Options;
using Board.Domain;
using Board.Infrastructure.DataAccess.Contexts.AdvertImages.Repositories;
using Board.Infrastructure.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RedLockNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.DataAccess.Contexts.AdvertFavorites.Repositories
{
    /// <inheritdoc />
    public class AdvertFavoriteRepository : IAdvertFavoriteRepository
    {
        private readonly IRepository<AdvertFavorite> _repository;
        private readonly ILogger<AdvertFavoriteRepository> _logger;
        private readonly IMapper _mapper;
        private readonly IDistributedLockFactory _distributedLockFactory;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly AdvertFavoriteAddLockOptions _addLockOptions;
        private readonly Contracts.Options.CookieOptions _cookieOptions;

        public AdvertFavoriteRepository(IRepository<AdvertFavorite> repository, ILogger<AdvertFavoriteRepository> logger, IMapper mapper,
            IDistributedLockFactory distributedLockFactory, IOptions<AdvertFavoriteAddLockOptions> addLockOptionsAccessor, IHttpContextAccessor contextAccessor,
            IOptions<Contracts.Options.CookieOptions> cookieOptionsAccessor)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _distributedLockFactory = distributedLockFactory;
            _addLockOptions = addLockOptionsAccessor.Value;
            _contextAccessor = contextAccessor;
            _cookieOptions = cookieOptionsAccessor.Value;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<Guid>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение списка избранных обьявлений пользователя с ID: {2}",
                nameof(AdvertFavoriteRepository), nameof(GetAllByUserIdAsync), userId);

            return await _repository.GetAll()
                .Where(a => a.UserId == userId)
                .Select(a => a.AdvertId)
                .ToListAsync(cancellation);
        }

        /// <inheritdoc />
        public async Task<Guid> AddIfNotExistsAsync(Guid advertId, Guid userId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Добавление обьявления с ID: {2} в избранное пользователя с ID: {3}",
                nameof(AdvertFavoriteRepository), nameof(AddIfNotExistsAsync), advertId, userId);

            var newAdvertFavorite = new AdvertFavorite { AdvertId = advertId, UserId = userId };

            var resource = $"{_addLockOptions.AdvertFavoriteAddKey}_{advertId}_{userId}";
            await using (var redLock = await _distributedLockFactory.CreateLockAsync(resource, _addLockOptions.Expire, _addLockOptions.Wait, _addLockOptions.Retry, cancellation))
            {
                if (redLock.IsAcquired)
                {
                    var advertFavorite = await _repository.GetAll()
                        .Where(af => af.AdvertId == advertId)
                        .Where(af => af.UserId == userId)
                        .FirstOrDefaultAsync(cancellation);

                    if (advertFavorite != null)
                    {
                        throw new ArgumentException("Обьявление уже находится в избранном.");
                    }

                    await _repository.AddAsync(newAdvertFavorite, cancellation);
                }
            }

            if(newAdvertFavorite.Id == Guid.Empty)
            {
                throw new InvalidOperationException($"Не удалось получить доступ к заблокированному ресурсу в течение {_addLockOptions.Wait}");
            }
         
            return newAdvertFavorite.Id;
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Guid advertId, Guid userId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Удаление из избранного пользователя с ID: {2} обьявления с ID: {3}",
                nameof(AdvertFavoriteRepository), nameof(DeleteAsync), userId, advertId);

            var advertFavorite = await _repository.GetAll()
                .Where(af => af.AdvertId == advertId && af.UserId == userId)
                .FirstOrDefaultAsync(cancellation);
            if (advertFavorite == null) 
            {
                throw new KeyNotFoundException($"В избранном пользователя с ID: {userId} не найдено указанное обьявление с ID: {advertId}");
            }

            await _repository.DeleteAsync(advertFavorite, cancellation);
        }


        public void AddToCookieIfNotExists(Guid advertId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Добавление обьявления c ID: {2} в избранное в cookie анонимного пользователя.",
                nameof(AdvertFavoriteRepository), nameof(AddToCookieIfNotExists), advertId);


            var advertFavoriteIds = new List<Guid>();
            var cookie = _contextAccessor.HttpContext.Request.Cookies[_cookieOptions.AnonymousFavoriteKey];
            try
            {
                advertFavoriteIds = JsonConvert.DeserializeObject<List<Guid>>(cookie);
            }
            catch (JsonException e)
            {
            }

            if (!advertFavoriteIds.Contains(advertId))
            {
                advertFavoriteIds.Add(advertId);
            }

            _contextAccessor.HttpContext.Response.Cookies.Append(_cookieOptions.AnonymousFavoriteKey, JsonConvert.SerializeObject(advertFavoriteIds));
        }

        public void DeleteFromCookie(Guid advertId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Удаление обьявления с ID: {2} из cookie из избранного анонимного пользователя.",
                nameof(AdvertFavoriteRepository), nameof(DeleteFromCookie), advertId);

            var advertFavoriteIds = new List<Guid>();
            var cookie = _contextAccessor.HttpContext.Request.Cookies[_cookieOptions.AnonymousFavoriteKey];
            try
            {
                advertFavoriteIds = JsonConvert.DeserializeObject<List<Guid>>(cookie);
            }
            catch (JsonException e)
            {
            }
            advertFavoriteIds.Remove(advertId);

            _contextAccessor.HttpContext.Response.Cookies.Append(_cookieOptions.AnonymousFavoriteKey, JsonConvert.SerializeObject(advertFavoriteIds));
        }

        public List<Guid> GetAllFromCookie(CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение списка избранных обьявлений из cookie анонимного пользователя.",
            nameof(AdvertFavoriteRepository), nameof(GetAllFromCookie));

            var advertFavoriteIds = new List<Guid>();
            var cookie = _contextAccessor.HttpContext.Request.Cookies[_cookieOptions.AnonymousFavoriteKey];
            try
            {
                return JsonConvert.DeserializeObject<List<Guid>>(cookie);
            }
            catch (JsonException e)
            {
            }

            return null;
        }
    }
}
