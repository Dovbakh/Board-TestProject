using Board.Application.AppData.Contexts.Posts.Repositories;
using Board.Contracts.Contexts.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.DataAccess.Contexts.Posts.Repositories
{
    public class AdvertRepository : IAdvertRepository
    {
        public Task<int> AddAsync(AdvertAddRequest addRequest, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<AdvertSummary>> GetAllAsync(int take, int skip, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<AdvertSummary>> GetAllFilteredAsync(AdvertFilterRequest filter, int take, int skip, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task<AdvertDetails> GetByIdAsync(int id, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(int id, AdvertUpdateRequest updateRequest, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }
    }
}
