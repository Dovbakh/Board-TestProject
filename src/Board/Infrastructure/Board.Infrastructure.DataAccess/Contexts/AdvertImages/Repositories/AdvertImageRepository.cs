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
        public async Task<AdvertImageDto> GetByIdAsync(Guid id, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение записи с указанием картинки обьявления по ID: {2} ",
                nameof(AdvertImageRepository), nameof(GetByIdAsync), id);

            var advert = await _repository.GetAll()
                .Where(a => a.Id == id)
                .ProjectTo<AdvertImageDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellation);

            return advert;
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
        public async Task DeleteAsync(Guid id, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Удаление записи с указанием картинки обьявления с ID: {2}",
                nameof(AdvertImageRepository), nameof(DeleteAsync), id);

            var existingEntity = await _repository.GetByIdAsync(id, cancellation);
            if(existingEntity == null) 
            {
                throw new KeyNotFoundException($"Не найдена запись с указанием картинки обьявления с ID: {id}");
            }

            await _repository.DeleteAsync(existingEntity, cancellation);
        }

        /// <inheritdoc />
        public async Task DeleteByFileIdAsync(Guid fileId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Удаление записи с указанием картинки обьявления с ID картинки: {2}",
                nameof(AdvertImageRepository), nameof(DeleteByFileIdAsync), fileId);

            var existingEntity = await _repository.GetAll()
                .Where(a => a.ImageId == fileId)
                .FirstOrDefaultAsync(cancellation);        
            
            if (existingEntity == null)
            {
                throw new KeyNotFoundException($"Не найдена запись с указанием картинки обьявления с ID картинки: {fileId}");
            }

            await _repository.DeleteAsync(existingEntity, cancellation);
        }
    }
}
