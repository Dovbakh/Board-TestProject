using AutoMapper;
using AutoMapper.QueryableExtensions;
using Board.Application.AppData.Contexts.AdvertImages.Repositories;
using Board.Contracts.Contexts.AdvertImages;
using Board.Domain;
using Board.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.DataAccess.Contexts.AdvertImages.Repositories
{
    /// <inheritdoc />
    public class AdvertImageRepository : IAdvertImageRepository
    {
        private readonly IRepository<AdvertImage> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<AdvertImageRepository> _logger;

        public AdvertImageRepository(IRepository<AdvertImage> repository, IMapper mapper, ILogger<AdvertImageRepository> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<AdvertImageDto>> GetAllByAdvertIdAsync(Guid advertId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение списка записей с указанием картинок обьявления по ID обьявления: {2} ",
                nameof(AdvertImageRepository), nameof(GetAllByAdvertIdAsync), advertId);

            var advertList = await _repository.GetAll()
                .Where(a => a.AdvertId == advertId)
                .ProjectTo<AdvertImageDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellation);

            return advertList;
        }

        /// <inheritdoc />
        public async Task<AdvertImageDto> GetByIdAsync(Guid advertImageId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение записи с указанием картинки обьявления по ID: {2} ",
                nameof(AdvertImageRepository), nameof(GetByIdAsync), advertImageId);

            var advert = await _repository.GetAll()
                .Where(a => a.Id == advertImageId)
                .ProjectTo<AdvertImageDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellation);

            return advert;
        }

        /// <inheritdoc />
        public async Task<bool> IsExists(Guid advertId, Guid imageId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Проверка наличия записи с указанием картинки с ID: {2} для обьявления с ID: {3} ",
                nameof(AdvertImageRepository), nameof(GetByIdAsync), imageId, advertId);

            var isExists = await _repository.GetAll()
                .AnyAsync(a => a.AdvertId == advertId && a.ImageId == imageId, cancellation);

            return isExists;
        }

        /// <inheritdoc />
        public async Task<Guid> AddAsync(AdvertImageAddRequest addRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Добавление записи с указанием картинки обьявления из модели {2}: {3}",
                nameof(AdvertImageRepository), nameof(AddAsync), nameof(addRequest), JsonConvert.SerializeObject(addRequest));

            var newEntity = _mapper.Map<AdvertImageAddRequest, AdvertImage>(addRequest);
            await _repository.AddAsync(newEntity, cancellation);

            return newEntity.Id;
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Guid advertImageId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Удаление записи с указанием картинки обьявления с ID: {2}",
                nameof(AdvertImageRepository), nameof(DeleteAsync), advertImageId);

            var existingEntity = await _repository.GetByIdAsync(advertImageId, cancellation);
            if(existingEntity == null) 
            {
                throw new KeyNotFoundException($"Не найдена запись с указанием картинки обьявления с ID: {advertImageId}");
            }

            await _repository.DeleteAsync(existingEntity, cancellation);
        }

        /// <inheritdoc />
        public async Task DeleteByFileIdAsync(Guid imageId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Удаление записи с указанием картинки обьявления с ID картинки: {2}",
                nameof(AdvertImageRepository), nameof(DeleteByFileIdAsync), imageId);

            var existingEntity = await _repository.GetAll()
                .Where(a => a.ImageId == imageId)
                .FirstOrDefaultAsync(cancellation);

            if (existingEntity == null)
            {
                throw new KeyNotFoundException($"Не найдена запись с указанием картинки обьявления с ID картинки: {imageId}");
            }

            await _repository.DeleteAsync(existingEntity, cancellation);
        }
    }
}
