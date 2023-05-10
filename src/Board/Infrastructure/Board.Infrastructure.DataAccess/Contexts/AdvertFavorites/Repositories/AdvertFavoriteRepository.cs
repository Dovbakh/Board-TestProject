using AutoMapper;
using AutoMapper.QueryableExtensions;
using Board.Application.AppData.Contexts.AdvertFavorites.Repositories;
using Board.Contracts.Contexts.AdvertFavorites;
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

namespace Board.Infrastructure.DataAccess.Contexts.AdvertFavorites.Repositories
{
    /// <inheritdoc />
    public class AdvertFavoriteRepository : IAdvertFavoriteRepository
    {
        private readonly IRepository<AdvertFavorite> _repository;
        private readonly ILogger<AdvertFavoriteRepository> _logger;
        private readonly IMapper _mapper;
        private readonly IDistributedLockFactory _distributedLockFactory;
        private const string CreateAdvertFavoriteKey = "CreateAdvertFavoriteKey_";

        public AdvertFavoriteRepository(IRepository<AdvertFavorite> repository, ILogger<AdvertFavoriteRepository> logger, IMapper mapper, IDistributedLockFactory distributedLockFactory)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _distributedLockFactory = distributedLockFactory;
        }

        /// <inheritdoc />
        public async Task<Guid> AddAsync(Guid advertId, Guid userId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Добавление обьявления с ID: {1} в избранное пользователя с ID: {2}",
                nameof(AddAsync), advertId, userId);

            var newAdvertFavorite = new AdvertFavorite { AdvertId = advertId, UserId = userId };

            var resource = $"{CreateAdvertFavoriteKey}_{advertId}_{userId}";
            var expiry = TimeSpan.FromSeconds(30);
            var wait = TimeSpan.FromSeconds(10);
            var retry = TimeSpan.FromSeconds(1);
            await using (var redLock = await _distributedLockFactory.CreateLockAsync(resource, expiry, wait, retry, cancellation))
            {
                if (redLock.IsAcquired)
                {
                    var existingAdvertFavorite = await _repository.GetAll()
                        .Where(af => af.AdvertId == advertId)
                        .Where(af => af.UserId == userId)
                        .FirstOrDefaultAsync(cancellation);

                    if (existingAdvertFavorite != null)
                    {
                        throw new ArgumentException("Обьявление уже находится в избранном.");
                    }

                    await _repository.AddAsync(newAdvertFavorite, cancellation);
                }
            }
         
            return newAdvertFavorite.Id;
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Guid advertFavoriteId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Удаление обьявления из избранного ID: {1}",
                nameof(DeleteAsync), advertFavoriteId);

            var advertFavorite = await _repository.GetByIdAsync(advertFavoriteId, cancellation);
            if (advertFavorite == null) 
            {
                throw new KeyNotFoundException($"Не найдено указанное обьявления в избранном с ID: {advertFavoriteId}");
            }

            await _repository.DeleteAsync(advertFavorite, cancellation);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<AdvertFavoriteSummary>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение списка избранных обьявлений пользователя с ID: {1}",
                nameof(GetAllByUserIdAsync), userId);

            var adverts = await _repository.GetAll()
                .Where(a => a.UserId == userId)
                .Include(a => a.Advert)
                .ProjectTo<AdvertFavoriteSummary>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellation);

            return adverts;
        }
    }
}
