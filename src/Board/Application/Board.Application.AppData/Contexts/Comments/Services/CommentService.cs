using Board.Contracts.Contexts.Comments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Comments.Services
{
    public class CommentService : ICommentService
    {
        public Task<int> AddAsync(CommentAddRequest addRequest, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<CommentDetails>> GetAllAsync(int? offset, int? count, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<CommentDetails>> GetAllFilteredAsync(CommentFilterRequest filterRequest, int? offset, int? count, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task<CommentDetails> GetByIdAsync(int id, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(int id, CommentUpdateRequest updateRequest, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }
    }
}
