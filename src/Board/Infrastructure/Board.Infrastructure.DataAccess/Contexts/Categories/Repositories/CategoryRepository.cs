using Board.Application.AppData.Contexts.Categories.Repositories;
using Board.Contracts.Contexts.Categories;
using Board.Contracts.Contexts.Comments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.DataAccess.Contexts.Categories.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        public Task<int> AddAsync(CategoryDetails categoryDto, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<CategoryDetails>> GetAllAsync(int take, int skip, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<CategoryDetails>> GetAllFilteredAsync(CategoryFilterRequest filterRequest, int take, int skip, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task<CategoryDetails> GetByIdAsync(int id, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(int id, CommentUpdateRequest updateRequest, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }
    }
}
