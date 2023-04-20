using AutoMapper;
using AutoMapper.QueryableExtensions;
using Board.Application.AppData.Contexts.Comments.Repositories;
using Board.Contracts.Contexts.Adverts;
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

namespace Board.Infrastructure.DataAccess.Contexts.Comments.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly IRepository<Comment> _repository;
        private readonly IMapper _mapper;

        public CommentRepository(IRepository<Comment> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IReadOnlyCollection<CommentDetails>> GetAllAsync(int offset, int limit, CancellationToken cancellation)
        {
            var existingDtoList = await _repository.GetAll()
                .ProjectTo<CommentDetails>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellation);

            return existingDtoList;
        }

        public async Task<IReadOnlyCollection<CommentDetails>> GetAllFilteredAsync(CommentFilterRequest filterRequest, int offset, int limit, CancellationToken cancellation)
        {
            var query = _repository.GetAll();


            if (filterRequest.UserId.HasValue)
            {
                query = query.Where(a => a.UserId == filterRequest.UserId);
            }
            if (filterRequest.AuthorId.HasValue)
            {
                query = query.Where(a => a.AuthorId == filterRequest.AuthorId);
            }
            if (filterRequest.AdvertisementId.HasValue)
            {
                query = query.Where(a => a.AdvertisementId == filterRequest.AdvertisementId);
            }

            if (!string.IsNullOrWhiteSpace(filterRequest.Text))
            {
                query = query.Where(p => p.Text.ToLower().Contains(filterRequest.Text.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(filterRequest.SortBy))
            {
                switch (filterRequest.SortBy)
                {
                    case "date":
                        query = filterRequest.OrderDesc == 1 ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt);
                        break;
                    case "rating":
                        query = filterRequest.OrderDesc == 1 ? query.OrderByDescending(p => p.Rating) : query.OrderBy(p => p.Rating);
                        break;
                    default:
                        break;
                }
            }

            var existingDtoList = await query
                .Include(c => c.Author)
                .ProjectTo<CommentDetails>(_mapper.ConfigurationProvider)
                .Skip(offset)
                .Take(limit)
                .ToListAsync(cancellation);


            return existingDtoList;
        }

        public async Task<CommentDetails> GetByIdAsync(Guid commentId, CancellationToken cancellation)
        {
            var existingDto = await _repository.GetAll()
                .Where(c => c.Id == commentId)
                .Include(c => c.Author)
                .ProjectTo<CommentDetails>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellation);

            return existingDto;
        }

        public async Task<Guid> AddAsync(CommentAddRequest addRequest, CancellationToken cancellation)
        {
            var newEntity = _mapper.Map<CommentAddRequest, Comment>(addRequest);
            await _repository.AddAsync(newEntity, cancellation);

            return newEntity.Id;
        }

        public async Task<CommentDetails> UpdateAsync(Guid commentId, CommentUpdateRequest updateRequest, CancellationToken cancellation)
        {
            var existingEntity = await _repository.GetByIdAsync(commentId, cancellation);
            if (existingEntity == null)
            {
                throw new KeyNotFoundException();
            }

            var updatedEntity = _mapper.Map<CommentUpdateRequest, Comment>(updateRequest, existingEntity);
            await _repository.UpdateAsync(updatedEntity, cancellation);
            var updatedDto = _mapper.Map<Comment, CommentDetails>(updatedEntity);

            return updatedDto;
        }

        public async Task DeleteAsync(Guid commentId, CancellationToken cancellation)
        {
            var existingEntity = await _repository.GetByIdAsync(commentId, cancellation);
            if (existingEntity == null)
            {
                throw new KeyNotFoundException();
            }

            await _repository.DeleteAsync(existingEntity, cancellation);
        }
    }
}
