using Board.Application.AppData.Contexts.Categories.Repositories;
using Board.Contracts.Contexts.Categories;
using System;
using System.Collections.Generic;
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

        public Task<int> AddAsync(CategoryDetails categoryDto, CancellationToken cancellation)
        {
            return _categoryRepository.AddAsync(categoryDto, cancellation);
        }

        public Task DeleteAsync(int id, CancellationToken cancellation)
        {
            return _categoryRepository.DeleteAsync(id, cancellation);

        }

        public Task<IReadOnlyCollection<CategoryDetails>> GetAllAsync(CancellationToken cancellation)
        {
            return _categoryRepository.GetAllAsync(0, 0, cancellation);
        }

        public Task<CategoryDetails> GetByIdAsync(int id, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(int id, CategoryDetails categoryDto, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }
    }
}
