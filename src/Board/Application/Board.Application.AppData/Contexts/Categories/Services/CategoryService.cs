using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Board.Application.AppData.Contexts.Adverts.Helpers;
using Board.Application.AppData.Contexts.Categories.Repositories;
using Board.Contracts.Contexts.Categories;
using Board.Domain;
using FluentValidation;
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
            _logger.LogInformation("{0} -> Получение всех категорий.", nameof(GetAllAsync));

            return _categoryRepository.GetAllAsync(cancellation);
        }

        /// <inheritdoc />
        public Task<IReadOnlyCollection<CategorySummary>> GetAllFilteredAsync(CategoryFilterRequest filterRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение всех категорий по фильтру {1}: {2}.",
                nameof(GetAllFilteredAsync), nameof(CategoryFilterRequest), JsonConvert.SerializeObject(filterRequest));

            return _categoryRepository.GetAllFilteredAsync(filterRequest, cancellation);
        }

        /// <inheritdoc />
        public async Task<CategoryDetails> GetByIdAsync(Guid id, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение категории по ID: {1} ",
                nameof(GetByIdAsync), id);

            var categoryDetails = await _categoryRepository.GetByIdAsync(id, cancellation);
            if(categoryDetails == null) 
            {
                throw new KeyNotFoundException($"Не найдена категория с ID: {id} ");
            }


            return categoryDetails;
        }

        /// <inheritdoc />
        public  Task<Guid> CreateAsync(CategoryAddRequest createRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Создание категории из модели {1}: {2}",
                nameof(CreateAsync), nameof(CategoryAddRequest), JsonConvert.SerializeObject(createRequest));

            var validationResult = _categoryAddValidator.Validate(createRequest);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException($"Модель создания категории не прошла валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
            }

            var newCategoryId = _categoryRepository.AddAsync(createRequest, cancellation);

            return newCategoryId;
        }

        /// <inheritdoc />
        public async Task<CategoryDetails> UpdateAsync(Guid categoryId, CategoryUpdateRequest updateRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Обновление категории c ID: {1} из модели {2}: {3}",
                nameof(UpdateAsync), categoryId, nameof(CategoryUpdateRequest), JsonConvert.SerializeObject(updateRequest));

            var validationResult = _categoryUpdateValidator.Validate(updateRequest);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException($"Модель обновлеиня категории не прошла валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
            }

            var updatedDto = await _categoryRepository.UpdateAsync(categoryId, updateRequest, cancellation);

            return updatedDto;
        }

        /// <inheritdoc />
        public Task DeleteAsync(Guid categoryId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Удаление категории с ID: {1}",
                nameof(DeleteAsync), categoryId);

            return _categoryRepository.DeleteAsync(categoryId, cancellation);
        }
    }
}
