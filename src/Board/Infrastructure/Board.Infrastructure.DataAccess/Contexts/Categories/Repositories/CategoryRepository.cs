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
            return await _repository.GetAll().ProjectTo<CategorySummary>(_mapper.ConfigurationProvider).ToListAsync(cancellation);
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


            return await query.ProjectTo<CategorySummary>(_mapper.ConfigurationProvider).ToListAsync(cancellation);
  
        }

        public async Task<CategoryDetails> GetByIdAsync(Guid id, CancellationToken cancellation)
        {
            return await _repository.GetAll().Where(c => c.Id == id).ProjectTo<CategoryDetails>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellation);
        }

        public async Task<Guid> AddAsync(Category entity, CancellationToken cancellation)
        {
            await _repository.AddAsync(entity, cancellation);
            
            return entity.Id;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellation)
        {
            var existingEntity = await _repository.GetByIdAsync(id, cancellation);
            if (existingEntity == null)
            {
                throw new KeyNotFoundException();
            }
            
            await _repository.DeleteAsync(existingEntity, cancellation);
        }

        public async Task UpdateAsync(Category entity, CancellationToken cancellation)
        {
            var existingEntity = await _repository.GetByIdAsync(entity.Id, cancellation);
            if (existingEntity == null)
            {
                throw new KeyNotFoundException();
            }

            await _repository.UpdateAsync(entity, cancellation);
        }
    }
}
