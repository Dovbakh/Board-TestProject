﻿using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Board.Application.AppData.Contexts.Categories.Repositories;
using Board.Contracts.Contexts.Categories;
using Board.Domain;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Categories.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public Task<IReadOnlyCollection<CategorySummary>> GetAllAsync(CancellationToken cancellation)
        {
            return _categoryRepository.GetAllAsync(cancellation);
        }

        public Task<CategoryDetails> GetByIdAsync(Guid id, CancellationToken cancellation)
        {
            return _categoryRepository.GetByIdAsync(id, cancellation);
        }

        public async Task<Guid> CreateAsync(CategoryCreateRequest createRequest, CancellationToken cancellation)
        {
            var entity = _mapper.Map<CategoryCreateRequest, Category>(createRequest);
            var entityId = await _categoryRepository.AddAsync(entity, cancellation);

            return entityId;
        }

        public Task DeleteAsync(Guid id, CancellationToken cancellation)
        {
            return _categoryRepository.DeleteAsync(id, cancellation);
        }


        public async Task<CategoryDetails> UpdateAsync(Guid id, CategoryUpdateRequest updateRequest, CancellationToken cancellation)
        {
            var entity = _mapper.Map<CategoryUpdateRequest, Category>(updateRequest);
            entity.Id = id;
            await _categoryRepository.UpdateAsync(entity, cancellation);

            var dto = _mapper.Map<Category, CategoryDetails>(entity);
            return dto;
        }
    }
}
