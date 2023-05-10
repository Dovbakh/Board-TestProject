using Board.Application.AppData.Contexts.AdvertFavorites.Repositories;
using Board.Contracts.Contexts.AdvertFavorites;
using Board.Contracts.Contexts.Adverts;
using Board.Domain;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.AdvertFavorites.Services
{
    /// <inheritdoc />
    public class AdvertFavoriteService : IAdvertFavoriteService
    {
        private readonly IAdvertFavoriteRepository _advertFavoriteRepository;
        private readonly ILogger<AdvertFavoriteService> _logger;

        public AdvertFavoriteService(IAdvertFavoriteRepository advertFavoriteRepository, ILogger<AdvertFavoriteService> logger)
        {
            _advertFavoriteRepository = advertFavoriteRepository;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<Guid> AddAsync(Guid advertId, Guid userId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Добавление обьявления с ID: {1} в избранное пользователя с ID: {2}",
                nameof(AddAsync), advertId, userId);

            var advertFavoriteId = await _advertFavoriteRepository.AddAsync(advertId, userId, cancellation);

            return advertFavoriteId;
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Guid advertFavoriteId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Удаление обьявления из избранного ID: {1}",
                nameof(DeleteAsync), advertFavoriteId);

            await _advertFavoriteRepository.DeleteAsync(advertFavoriteId, cancellation);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<AdvertFavoriteSummary>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение списка избранных обьявлений пользователя с ID: {1}",
                nameof(GetAllByUserIdAsync), userId);

            var adverts = await _advertFavoriteRepository.GetAllByUserIdAsync(userId, cancellation);

            return adverts;
        }
    }
}
