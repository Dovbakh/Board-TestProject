using AutoMapper;
using AutoMapper.QueryableExtensions;
using Board.Application.AppData.Contexts.Categories.Repositories;
using Board.Contracts.Contexts.Categories;
using Board.Contracts.Contexts.Comments;
using Board.Domain;
using Board.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.DataAccess.Contexts.Categories.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IRepository<Category> _repository;
        private readonly IMapper _mapper;

        public CategoryRepository(Repository.IRepository<Category> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IReadOnlyCollection<CategorySummary>> GetAllAsync(CancellationToken cancellation)
        {
            var existingDtoList = await _repository.GetAll()
                .ProjectTo<CategorySummary>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellation);

            return existingDtoList;
        }

        public async Task<IReadOnlyCollection<CategorySummary>> GetAllFilteredAsync(CategoryFilterRequest filterRequest, CancellationToken cancellation)
        {
            var query = _repository.GetAll();

            if (!string.IsNullOrWhiteSpace(filterRequest.Name))
            {
                query = query.Where(p => p.Name.ToLower().Contains(filterRequest.Name.ToLower()));
            }

            if (filterRequest.ParentId.HasValue)
            {
                query = query.Where(a => a.ParentId == filterRequest.ParentId);
            }

            var existingDtoList = await query
                .ProjectTo<CategorySummary>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellation);


            return existingDtoList;
        }

        public async Task<CategoryDetails> GetByIdAsync(Guid categoryId, CancellationToken cancellation)
        {
            var existingDto = await _repository.GetAll()
                .Where(c => c.Id == categoryId)
                .ProjectTo<CategoryDetails>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellation);

            return existingDto;
        }

        public async Task<Guid> AddAsync(CategoryCreateRequest createRequest, CancellationToken cancellation)
        {
            var newEntity = _mapper.Map<CategoryCreateRequest, Category>(createRequest);
            await _repository.AddAsync(newEntity, cancellation);
            
            return newEntity.Id;
        }

        public async Task<CategoryDetails> UpdateAsync(Guid categoryId, CategoryUpdateRequest updateRequest, CancellationToken cancellation)
        {
            var existingEntity = await _repository.GetByIdAsync(categoryId, cancellation);
            if (existingEntity == null)
            {
                throw new KeyNotFoundException();
            }

            var updatedEntity = _mapper.Map<CategoryUpdateRequest, Category>(updateRequest, existingEntity);
            await _repository.UpdateAsync(updatedEntity, cancellation);
            var updatedDto = _mapper.Map<Category, CategoryDetails>(updatedEntity);

            return updatedDto;
        }

        public async Task DeleteAsync(Guid categoryId, CancellationToken cancellation)
        {
            var existingEntity = await _repository.GetByIdAsync(categoryId, cancellation);
            if (existingEntity == null)
            {
                throw new KeyNotFoundException();
            }

            await _repository.DeleteAsync(existingEntity, cancellation);
        }
    }
}
