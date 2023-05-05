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
            _logger.LogInformation("{0} -> Получение всех обьявлений с параметрами: {1} = {2}, {3} = {4}",
                nameof(GetAllAsync), nameof(offset), offset, nameof(limit), limit);

            var advertDetailsList = await _repository.GetAll()
                .OrderByDescending(a => a.CreatedAt)
                .Include(a => a.AdvertImages)
                .ProjectTo<AdvertSummary>(_mapper.ConfigurationProvider)
                .Skip(offset)
                .Take(limit)
                .ToListAsync(cancellation);

            return advertDetailsList;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<AdvertSummary>> GetAllFilteredAsync(AdvertFilterRequest filterRequest, int offset, int limit, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение всех обьявлений по фильтру с параметрами: {1} = {2}, {3} = {4}, {5} = {6}",
                nameof(GetAllFilteredAsync), nameof(offset), offset, nameof(limit), limit, nameof(filterRequest),
                JsonConvert.SerializeObject(filterRequest));

            var query = _repository.GetAll();

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

            if (filterRequest.minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= filterRequest.minPrice);
            }

            if (filterRequest.maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= filterRequest.maxPrice);
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

            var advertDetailsList = await query
                .Include(a => a.AdvertImages)
                .ProjectTo<AdvertSummary>(_mapper.ConfigurationProvider)
                .Skip(offset)
                .Take(limit)
                .ToListAsync(cancellation);


            return advertDetailsList;
        }

        /// <inheritdoc />
        public async Task<AdvertDetails> GetByIdAsync(Guid advertId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение обьявления по ID: {1} ",
                nameof(GetByIdAsync), advertId);

            var advertDetails = await _repository.GetAll()
                .Where(a => a.Id == advertId)
                .Include(a => a.Category)
                .Include(a => a.AdvertImages)
                .ProjectTo<AdvertDetails>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellation);

            return advertDetails;
        }

        /// <inheritdoc />
        public async Task<Guid> AddAsync(AdvertAddRequest addRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Создание обьявления из модели {1}: {2}",
                nameof(AddAsync), nameof(AdvertAddRequest), JsonConvert.SerializeObject(addRequest));

            var advert = _mapper.Map<AdvertAddRequest, Advert>(addRequest);
            await _repository.AddAsync(advert, cancellation);

            return advert.Id;
        }

        /// <inheritdoc />
        public async Task<AdvertDetails> UpdateAsync(Guid advertId, AdvertUpdateRequest updateRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Обновление обьявления c ID: {1} из модели {2}: {3}",
                nameof(UpdateAsync), advertId, nameof(AdvertUpdateRequest), JsonConvert.SerializeObject(updateRequest));

            var existingAdvert = await _repository.GetAll()
                .Where(a => a.Id == advertId)
                .Include(a => a.Category)
                .Include(a => a.AdvertImages)
                .FirstOrDefaultAsync(cancellation);
            if (existingAdvert == null)
            {
                throw new KeyNotFoundException($"Не найдено обьявление с ID: {advertId}");
            }

            var updatedAdvert = _mapper.Map<AdvertUpdateRequest, Advert>(updateRequest, existingAdvert);
            await _repository.UpdateAsync(updatedAdvert, cancellation);

            var updatedAdvertDetails = _mapper.Map<Advert, AdvertDetails>(updatedAdvert);

            return updatedAdvertDetails;
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Guid advertId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Удаление обьявления с ID: {1}",
                nameof(DeleteAsync), advertId);

            var existingAdvert = await _repository.GetByIdAsync(advertId, cancellation);
            if (existingAdvert == null)
            {
                throw new KeyNotFoundException($"Не найдено обьявление с ID: {advertId}");
            }

            await _repository.DeleteAsync(existingAdvert, cancellation);
        }

        /// <inheritdoc />
        public async Task SoftDeleteAsync(Guid advertId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Мягкое удаление обьявления с ID: {1}",
                nameof(SoftDeleteAsync), advertId);

            var existingAdvert = await _repository.GetByIdAsync(advertId, cancellation);
            if (existingAdvert == null)
            {
                throw new KeyNotFoundException($"Не найдено обьявление с ID: {advertId}");
            }

            existingAdvert.isActive = false;
            await _repository.UpdateAsync(existingAdvert, cancellation);
        }
    }
}
