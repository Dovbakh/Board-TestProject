using Board.Application.AppData.Contexts.Posts.Repositories;
using Board.Contracts.Contexts.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.DataAccess.Contexts.Posts.Repositories
{
    public class PostRepository : IPostRepository
    {
        public Task<int> AddAsync(PostAddRequest addRequest, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<PostSummary>> GetAllAsync(int take, int skip, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<PostSummary>> GetAllFilteredAsync(PostFilterRequest filter, int take, int skip, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task<PostDetails> GetByIdAsync(int id, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(int id, PostUpdateRequest updateRequest, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }
    }
}
