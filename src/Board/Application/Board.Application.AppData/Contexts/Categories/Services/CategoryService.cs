using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Board.Application.AppData.Contexts.AdvertFavorites.Services;
using Board.Application.AppData.Contexts.Adverts.Helpers;
using Board.Application.AppData.Contexts.Categories.Repositories;
using Board.Contracts.Contexts.Categories;
using Board.Domain;
using FluentValidation;
using MassTransit.Configuration;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Categories.Services
{
    /// <inheritdoc />
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper; 
        private readonly ILogger<CategoryService> _logger;
        private readonly IValidator<CategoryAddRequest> _categoryAddValidator;
        private readonly IValidator<CategoryUpdateRequest> _categoryUpdateValidator;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper, ILogger<CategoryService> logger, IValidator<CategoryAddRequest> categoryAddValidator, 
            IValidator<CategoryUpdateRequest> categoryUpdateValidator)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _logger = logger;
            _categoryAddValidator = categoryAddValidator;
            _categoryUpdateValidator = categoryUpdateValidator;
        }

        /// <inheritdoc />
        public Task<IReadOnlyCollection<CategorySummary>> GetAllAsync(CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение всех категорий.",
                nameof(CategoryService), nameof(GetAllAsync));

            return _categoryRepository.GetAllAsync(cancellation);
        }

        /// <inheritdoc />
        public Task<IReadOnlyCollection<CategorySummary>> GetAllFilteredAsync(CategoryFilterRequest filterRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение всех категорий по фильтру {2}: {3}",
                nameof(CategoryService), nameof(GetAllFilteredAsync), nameof(CategoryFilterRequest), JsonConvert.SerializeObject(filterRequest));

            return _categoryRepository.GetAllFilteredAsync(filterRequest, cancellation);
        }

        /// <inheritdoc />
        public async Task<CategoryDetails> GetByIdAsync(Guid categoryId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение категории по ID: {2} ",
                nameof(CategoryService), nameof(GetByIdAsync), categoryId);

            var category = await _categoryRepository.GetByIdAsync(categoryId, cancellation);
            if(category == null) 
            {
                throw new KeyNotFoundException($"Не найдена категория с ID: {categoryId} ");
            }

            return category;
        }

        /// <inheritdoc />
        public  async Task<Guid> CreateAsync(CategoryAddRequest createRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Создание категории из модели {2}: {3}",
                nameof(CategoryService), nameof(CreateAsync), nameof(CategoryAddRequest), JsonConvert.SerializeObject(createRequest));

            await _categoryAddValidator.ValidateAndThrowAsync(createRequest, cancellation);

            var newCategoryId = await _categoryRepository.AddAsync(createRequest, cancellation);

            return newCategoryId;
        }

        /// <inheritdoc />
        public async Task<CategoryDetails> UpdateAsync(Guid categoryId, CategoryUpdateRequest updateRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Обновление категории c ID: {2} из модели {3}: {4}",
                nameof(CategoryService), nameof(UpdateAsync), categoryId, nameof(CategoryUpdateRequest), JsonConvert.SerializeObject(updateRequest));

            await _categoryUpdateValidator.ValidateAndThrowAsync(updateRequest, cancellation); 

            var updatedCategory = await _categoryRepository.UpdateAsync(categoryId, updateRequest, cancellation);

            return updatedCategory;
        }

        /// <inheritdoc />
        public Task DeleteAsync(Guid categoryId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Удаление категории с ID: {2}",
                nameof(CategoryService), nameof(DeleteAsync), categoryId);

            return _categoryRepository.DeleteAsync(categoryId, cancellation);
        }
    }
}
