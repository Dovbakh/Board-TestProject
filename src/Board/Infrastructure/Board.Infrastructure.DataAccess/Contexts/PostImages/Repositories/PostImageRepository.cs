using Board.Application.AppData.Contexts.PostImages.Repositories;
using Board.Contracts.Contexts.PostImages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.DataAccess.Contexts.PostImages.Repositories
{
    public class PostImageRepository : IPostImageRepository
    {
        public Task<int> AddAsync(PostImageAddRequest addRequest, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<PostImageItem>> GetAllByPostIdAsync(int postId, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task<PostImageItem> GetByIdAsync(int id, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }
    }
}
