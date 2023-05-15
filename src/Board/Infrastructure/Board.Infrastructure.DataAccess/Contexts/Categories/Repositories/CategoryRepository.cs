using AutoMapper;
using AutoMapper.QueryableExtensions;
using Board.Application.AppData.Contexts.Categories.Repositories;
using Board.Contracts.Contexts.Adverts;
using Board.Contracts.Contexts.Categories;
using Board.Contracts.Contexts.Comments;
using Board.Contracts.Options;
using Board.Domain;
using Board.Infrastructure.DataAccess.Contexts.AdvertViews.Repositories;
using Board.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private readonly CategoryOptions _categoryOptions;

        public CategoryRepository(Repository.IRepository<Category> repository, IMapper mapper, ICacheRepository cacheRepository, ILogger<CategoryRepository> logger, 
            IOptions<CategoryOptions> categoryOptionsAccessor)
        {
            _repository = repository;
            _mapper = mapper;
            _cacheRepository = cacheRepository;
            _logger = logger;
            _categoryOptions = categoryOptionsAccessor.Value;
        }

        public async Task<IReadOnlyCollection<CategorySummary>> GetAllAsync(CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение всех категорий.",
                 nameof(CategoryRepository), nameof(GetAllAsync));

            var categories = await _cacheRepository.GetById<IReadOnlyCollection<CategorySummary>>(_categoryOptions.CategoryListKey, cancellation);

            if (categories == null)
            {
                categories = await _repository.GetAll()
                    .Where(c => c.isActive == true)
                    .ProjectTo<CategorySummary>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellation);

                await _cacheRepository.SetWithSlidingTime(_categoryOptions.CategoryListKey, categories, TimeSpan.FromSeconds(60), cancellation);
            }

            return categories;
        }

        public async Task<IReadOnlyCollection<CategorySummary>> GetAllFilteredAsync(CategoryFilterRequest filterRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение всех категорий по фильтру {2}: {3}",
                 nameof(CategoryRepository), nameof(GetAllFilteredAsync), nameof(CategoryFilterRequest), JsonConvert.SerializeObject(filterRequest));

            var query = _repository.GetAll()
                .Where(c => c.isActive == true);

            if (!string.IsNullOrWhiteSpace(filterRequest.Name))
            {
                query = query.Where(p => p.Name.ToLower().Contains(filterRequest.Name.ToLower()));
            }

            if (filterRequest.ParentId.HasValue)
            {
                query = query.Where(a => a.ParentId == filterRequest.ParentId);
            }

            var categories = await query
                .ProjectTo<CategorySummary>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellation);

            return categories;
        }

        public async Task<CategoryDetails> GetByIdAsync(Guid categoryId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение категории с ID: {2}",
                 nameof(CategoryRepository), nameof(GetByIdAsync), categoryId);

            var category = await _repository.GetAll()
                .Where(c => c.Id == categoryId && c.isActive == true)
                .ProjectTo<CategoryDetails>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellation);

            return category;
        }

        public async Task<Guid> AddAsync(CategoryAddRequest createRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Создание категории из модели {2}: {3}",
                 nameof(CategoryRepository), nameof(AddAsync), nameof(CategoryAddRequest), JsonConvert.SerializeObject(createRequest));

            var newCategory = _mapper.Map<CategoryAddRequest, Category>(createRequest);
            await _repository.AddAsync(newCategory, cancellation);

            await _cacheRepository.DeleteAsync(_categoryOptions.CategoryListKey, cancellation);
            
            return newCategory.Id;
        }

        public async Task<CategoryDetails> UpdateAsync(Guid categoryId, CategoryUpdateRequest updateRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Обновление категории c ID: {2} из модели {3}: {4}",
                 nameof(CategoryRepository), nameof(UpdateAsync), categoryId, nameof(CategoryUpdateRequest), JsonConvert.SerializeObject(updateRequest));

            var category = await _repository.GetByIdAsync(categoryId, cancellation);
            if (category == null)
            {
                throw new KeyNotFoundException($"Не найдена категория с ID: {categoryId}");
            }

            var updatedCategory = _mapper.Map<CategoryUpdateRequest, Category>(updateRequest, category);
            await _repository.UpdateAsync(updatedCategory, cancellation);

            await _cacheRepository.DeleteAsync(_categoryOptions.CategoryListKey, cancellation);

            var updatedCategoryDto = _mapper.Map<Category, CategoryDetails>(updatedCategory);
            return updatedCategoryDto;
        }

        public async Task DeleteAsync(Guid categoryId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Удаление категории с ID: {2}",
                 nameof(CategoryRepository), nameof(DeleteAsync), categoryId);

            var category = await _repository.GetByIdAsync(categoryId, cancellation);
            if (category == null)
            {
                throw new KeyNotFoundException($"Не найдена категория с ID: {categoryId}");
            }

            await _repository.DeleteAsync(category, cancellation);
        }
    }
}
