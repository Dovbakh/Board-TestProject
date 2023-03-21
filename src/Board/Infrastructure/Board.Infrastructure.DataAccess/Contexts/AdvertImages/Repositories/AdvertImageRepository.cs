using Board.Application.AppData.Contexts.PostImages.Repositories;
using Board.Contracts.Contexts.PostImages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.DataAccess.Contexts.PostImages.Repositories
{
    public class AdvertImageRepository : IAdvertImageRepository
    {
        public Task<int> AddAsync(AdvertImageAddRequest addRequest, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<AdvertImageDto>> GetAllByPostIdAsync(int postId, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task<AdvertImageDto> GetByIdAsync(int id, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }
    }
}
