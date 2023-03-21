using Board.Application.AppData.Contexts.Posts.Services;
using Board.Contracts.Contexts.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Adverts.Services
{
    public class AdvertService : IAdvertService
    {
        public Task<int> AddAsync(AdvertAddRequest addRequest, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<AdvertSummary>> GetAllAsync(int? offset, int? count, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<AdvertSummary>> GetAllFilteredAsync(AdvertFilterRequest request, int? offset, int? count, CancellationToken cancellation)
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
