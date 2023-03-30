using Board.Application.AppData.Contexts.Categories.Repositories;
using Board.Contracts.Contexts.Categories;
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

        /// <summary>
        /// Инициализировать экземпляр <see cref="CategoryService"/>
        /// </summary>
        /// <param name="repository">Репозиторий для работы с <see cref="CategoryDto"/></param>
        public CategoryService(ICategoryRepository repository)
        {
            _categoryRepository = repository;
        }

        public Task<Guid> CreateAsync(CategoryCreateRequest createRequest, CancellationToken cancellation)
        {
            return _categoryRepository.AddAsync(new CategoryDetails { }, cancellation);
        }

        public Task DeleteAsync(Guid id, CancellationToken cancellation)
        {
            return _categoryRepository.DeleteAsync(id, cancellation);

        }

        public Task<IReadOnlyCollection<CategorySummary>> GetAllAsync(CancellationToken cancellation)
        {
            return Task<IReadOnlyCollection<CategorySummary>>.Run(() => (IReadOnlyCollection < CategorySummary > )new List<CategorySummary>());
            //return _categoryRepository.GetAllAsync(0, 0, cancellation);
        }

        public Task<CategoryDetails> GetByIdAsync(Guid id, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Guid id, CategoryUpdateRequest updateRequest, CancellationToken cancellation)
        {
            throw new NotImplementedException();

            
        }
    }
}
