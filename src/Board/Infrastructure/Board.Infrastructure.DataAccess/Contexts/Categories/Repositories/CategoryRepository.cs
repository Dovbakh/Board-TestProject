using AutoMapper;
using AutoMapper.QueryableExtensions;
using Board.Application.AppData.Contexts.Categories.Repositories;
using Board.Contracts.Contexts.Adverts;
using Board.Contracts.Contexts.Categories;
using Board.Contracts.Contexts.Comments;
using Board.Domain;
using Board.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.DataAccess.Contexts.Categories.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IRepository<Category> _repository;
        private readonly ICacheRepository _cacheRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryRepository> _logger;
        private const string CategoryListKey = "CategoryListKey_";

        public CategoryRepository(Repository.IRepository<Category> repository, IMapper mapper, ICacheRepository cacheRepository, ILogger<CategoryRepository> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _cacheRepository = cacheRepository;
            _logger = logger;
        }

        public async Task<IReadOnlyCollection<CategorySummary>> GetAllAsync(CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение всех категорий.", nameof(GetAllAsync));

            var categorySummaryList = (IReadOnlyCollection<CategorySummary>)await _cacheRepository.GetById(CategoryListKey, typeof(IReadOnlyCollection<CategorySummary>), cancellation); // TODO : чек
            if (categorySummaryList == null)
            {
                categorySummaryList = await _repository.GetAll()
               .ProjectTo<CategorySummary>(_mapper.ConfigurationProvider)
               .ToListAsync(cancellation);

                _logger.LogInformation("{0} -> Сохранение списка категорий в кэше с ключом {1}.", nameof(GetAllAsync), CategoryListKey);
                await _cacheRepository.SetWithSlidingTime(CategoryListKey, typeof(IReadOnlyCollection<CategorySummary>), categorySummaryList, TimeSpan.FromSeconds(60), cancellation);
            }
            return categorySummaryList;
        }

        public async Task<IReadOnlyCollection<CategorySummary>> GetAllFilteredAsync(CategoryFilterRequest filterRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение всех категорий по фильтру {1}: {2}.",
                nameof(GetAllFilteredAsync), nameof(CategoryFilterRequest), JsonConvert.SerializeObject(filterRequest));

            var query = _repository.GetAll();

            if (!string.IsNullOrWhiteSpace(filterRequest.Name))
            {
                query = query.Where(p => p.Name.ToLower().Contains(filterRequest.Name.ToLower()));
            }

            if (filterRequest.ParentId.HasValue)
            {
                query = query.Where(a => a.ParentId == filterRequest.ParentId);
            }

            var categorySummaryList = await query
                .ProjectTo<CategorySummary>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellation);


            return categorySummaryList;
        }

        public async Task<CategoryDetails> GetByIdAsync(Guid categoryId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение категории с ID: {1}",
                nameof(GetByIdAsync), categoryId);


            var categoryDetails = await _repository.GetAll()
                .Where(c => c.Id == categoryId)
                .ProjectTo<CategoryDetails>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellation);


            return categoryDetails;
        }

        public async Task<Guid> AddAsync(CategoryAddRequest createRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Создание категории из модели {1}: {2}",
                nameof(AddAsync), nameof(CategoryAddRequest), JsonConvert.SerializeObject(createRequest));


            var newCategory = _mapper.Map<CategoryAddRequest, Category>(createRequest);
            await _repository.AddAsync(newCategory, cancellation);

            await _cacheRepository.DeleteAsync(CategoryListKey, cancellation);
            
            return newCategory.Id;
        }

        public async Task<CategoryDetails> UpdateAsync(Guid categoryId, CategoryUpdateRequest updateRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Обновление категории c ID: {1} из модели {2}: {3}",
                nameof(UpdateAsync), categoryId, nameof(CategoryUpdateRequest), JsonConvert.SerializeObject(updateRequest));

            var existingCategory = await _repository.GetByIdAsync(categoryId, cancellation);
            if (existingCategory == null)
            {
                throw new KeyNotFoundException($"Не найдена категория с ID: {categoryId}");
            }

            var updatedCategory = _mapper.Map<CategoryUpdateRequest, Category>(updateRequest, existingCategory);
            await _repository.UpdateAsync(updatedCategory, cancellation);

            _logger.LogInformation("{0} -> Инвалидация списка категорий из кэша с ключом: {1}",
                nameof(UpdateAsync), CategoryListKey);
            await _cacheRepository.DeleteAsync(CategoryListKey, cancellation);

            var updatedCategoryDetails = _mapper.Map<Category, CategoryDetails>(updatedCategory);

            return updatedCategoryDetails;
        }

        public async Task DeleteAsync(Guid categoryId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Удаление категории с ID: {1}",
                nameof(DeleteAsync), categoryId);

            var existingEntity = await _repository.GetByIdAsync(categoryId, cancellation);
            if (existingEntity == null)
            {
                throw new KeyNotFoundException($"Не найдена категория с ID: {categoryId}");
            }

            await _repository.DeleteAsync(existingEntity, cancellation);
        }
    }
}
