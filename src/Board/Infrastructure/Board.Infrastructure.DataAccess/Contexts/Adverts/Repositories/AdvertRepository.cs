using AutoMapper;
using AutoMapper.Configuration.Annotations;
using AutoMapper.QueryableExtensions;
using Board.Application.AppData.Contexts.Adverts.Repositories;
using Board.Contracts.Contexts.Adverts;
using Board.Contracts.Contexts.Categories;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Board.Infrastructure.DataAccess.Contexts.Adverts.Repositories
{
    /// <inheritdoc />
    public class AdvertRepository : IAdvertRepository
    {
        private readonly IRepository<Advert> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<AdvertRepository> _logger;

        public AdvertRepository(IRepository<Advert> repository, IMapper mapper, ILogger<AdvertRepository> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<AdvertSummary>> GetAllAsync(int offset, int limit, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение всех обьявлений с параметрами: {2} = {3}, {4} = {5}",
                nameof(AdvertRepository), nameof(GetAllAsync), nameof(offset), offset, nameof(limit), limit);

            var adverts = await _repository.GetAll()
                .Where(a => a.IsActive == true)
                .OrderByDescending(a => a.CreatedAt)               
                .Skip(offset)
                .Take(limit)
                .Include(a => a.AdvertImages)
                .ProjectTo<AdvertSummary>(_mapper.ConfigurationProvider)             
                .ToListAsync(cancellation);

            return adverts;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<AdvertSummary>> GetAllFilteredAsync(AdvertFilterRequest filterRequest, int offset, int limit, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение всех обьявлений по фильтру с параметрами: {2} = {3}, {4} = {5}, {6} = {7}",
                nameof(AdvertRepository), nameof(GetAllFilteredAsync), nameof(offset), offset, nameof(limit), limit, nameof(filterRequest),
                JsonConvert.SerializeObject(filterRequest));

            var query = _repository.GetAll()
                .Where(a => a.IsActive == true);

            if (filterRequest.UserId.HasValue)
            {
                query = query.Where(a => a.UserId == filterRequest.UserId);
            }
            if (filterRequest.CategoryId.HasValue)
            {
                query = query.Where(a => a.CategoryId == filterRequest.CategoryId);
            }

            if (!string.IsNullOrWhiteSpace(filterRequest.Text))
            {
                query = query.Where(p => p.Name.ToLower().Contains(filterRequest.Text.ToLower()) || p.Description.ToLower().Contains(filterRequest.Text.ToLower()));
            }

            if (filterRequest.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= filterRequest.MinPrice);
            }

            if (filterRequest.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= filterRequest.MaxPrice);
            }

            if (!string.IsNullOrWhiteSpace(filterRequest.SortBy))
            {
                switch (filterRequest.SortBy)
                {
                    case "date":
                        query = filterRequest.OrderDesc == 1 ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt);
                        break;
                    case "price":
                        query = filterRequest.OrderDesc == 1 ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price);
                        break;
                    default:
                        query = query.OrderByDescending(p => p.CreatedAt);
                        break;
                }
            }

            var adverts = await query
                .Skip(offset)
                .Take(limit)
                .Include(a => a.AdvertImages)
                .ProjectTo<AdvertSummary>(_mapper.ConfigurationProvider)              
                .ToListAsync(cancellation);

            return adverts;
        }

        /// <inheritdoc />
        public async Task<AdvertDetails> GetByIdAsync(Guid advertId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение обьявления по ID: {2}",
                nameof(AdvertRepository), nameof(GetByIdAsync), advertId);

            var advert = await _repository.GetAll()
                .Where(a => a.Id == advertId && a.IsActive == true)
                .Include(a => a.Category)
                .Include(a => a.AdvertImages)
                .ProjectTo<AdvertDetails>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellation);

            return advert;
        }

        public async Task<IReadOnlyCollection<AdvertSummary>> GetByListIdAsync(List<Guid> advertIds, int offset, int limit, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение списка обьявлений с ID из списка: {2}",
                nameof(AdvertRepository), nameof(GetByIdAsync), JsonConvert.SerializeObject(advertIds));

            var adverts = await _repository.GetAll()
                .Where(a => advertIds.Contains(a.Id))
                .OrderByDescending(a => a.CreatedAt)
                .Skip(offset)
                .Take(limit)
                .Include(a => a.AdvertImages)
                .ProjectTo<AdvertSummary>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellation);

            return adverts;
        }

        public async Task<IReadOnlyCollection<AdvertSummary>> GetFavoritesByUserIdAsync(Guid userId, int offset, int limit, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение списка избранных обьявлений для пользователя с ID: {2}",
                nameof(AdvertRepository), nameof(GetByIdAsync), JsonConvert.SerializeObject(userId));

            var adverts = await _repository.GetAll()
                .Where(a => a.AdvertFavorites.Any(af => af.UserId == userId))
                .OrderByDescending(a => a.CreatedAt)
                .Skip(offset)
                .Take(limit)
                .Include(a => a.AdvertImages)
                .ProjectTo<AdvertSummary>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellation);

            return adverts;
        }

        /// <inheritdoc />
        public async Task<Guid> GetUserIdAsync(Guid advertId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение ID пользователя, создавшего обьявления с ID: {2}",
                nameof(AdvertRepository), nameof(GetByIdAsync), advertId);

            var userId = await _repository.GetAll()
                .Where(a => a.Id == advertId && a.IsActive == true)
                .Select(a => a.UserId)
                .FirstOrDefaultAsync(cancellation);
            if(userId == null)
            {
                throw new KeyNotFoundException($"Не найдено обьявление с ID: {advertId}");
            }

            return userId;
        }

        /// <inheritdoc />
        public async Task<bool> IsExists(Guid advertId, CancellationToken cancellation)
        {
            var isExists = await _repository.GetAll()
                .AnyAsync(a => a.Id == advertId, cancellation);

            return isExists;              
        }

        /// <inheritdoc />
        public async Task<Guid> AddAsync(AdvertAddRequest addRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Создание обьявления из модели {2}: {3}",
                nameof(AdvertRepository), nameof(AddAsync), nameof(AdvertAddRequest), JsonConvert.SerializeObject(addRequest));

            var advert = _mapper.Map<AdvertAddRequest, Advert>(addRequest);
            await _repository.AddAsync(advert, cancellation);

            return advert.Id;
        }

        /// <inheritdoc />
        public async Task<AdvertDetails> UpdateAsync(Guid advertId, AdvertUpdateRequest updateRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Обновление обьявления c ID: {2} из модели {3}: {4}",
                nameof(AdvertRepository), nameof(UpdateAsync), advertId, nameof(AdvertUpdateRequest), JsonConvert.SerializeObject(updateRequest));

            var advert = await _repository.GetAll()
                .Where(a => a.Id == advertId)
                .Include(a => a.AdvertImages)
                .FirstOrDefaultAsync(cancellation);

            if (advert == null)
            {
                throw new KeyNotFoundException($"Не найдено обьявление с ID: {advertId}");
            }

            var updatedAdvert = _mapper.Map<AdvertUpdateRequest, Advert>(updateRequest, advert);
            await _repository.UpdateAsync(updatedAdvert, cancellation);
            var updatedAdvertDto = _mapper.Map<Advert, AdvertDetails>(updatedAdvert);

            return updatedAdvertDto;
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Guid advertId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Удаление обьявления с ID: {2}",
                nameof(AdvertRepository), nameof(DeleteAsync), advertId);

            var advert = await _repository.GetByIdAsync(advertId, cancellation);
            if (advert == null)
            {
                throw new KeyNotFoundException($"Не найдено обьявление с ID: {advertId}");
            }

            await _repository.DeleteAsync(advert, cancellation);
        }

        /// <inheritdoc />
        public async Task SoftDeleteAsync(Guid advertId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Мягкое удаление обьявления с ID: {2}",
                nameof(AdvertRepository), nameof(SoftDeleteAsync), advertId);

            var advert = await _repository.GetByIdAsync(advertId, cancellation);
            if (advert == null)
            {
                throw new KeyNotFoundException($"Не найдено обьявление с ID: {advertId}");
            }

            advert.IsActive = false;
            await _repository.UpdateAsync(advert, cancellation);
        }
    }
}
