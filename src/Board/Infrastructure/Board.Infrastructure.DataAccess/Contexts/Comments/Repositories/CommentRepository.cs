using AutoMapper;
using AutoMapper.QueryableExtensions;
using Board.Application.AppData.Contexts.Comments.Repositories;
using Board.Contracts.Contexts.Adverts;
using Board.Contracts.Contexts.Categories;
using Board.Contracts.Contexts.Comments;
using Board.Domain;
using Board.Contracts.Options;
using Board.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RedLockNet;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.DataAccess.Contexts.Comments.Repositories
{
    /// <inheritdoc />
    public class CommentRepository : ICommentRepository
    {
        private readonly IRepository<Comment> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<ILogger> _logger;
        private readonly IDistributedLockFactory _distributedLockFactory;
        private readonly CommentAddLockOptions _addLockOptions;

        public CommentRepository(IRepository<Comment> repository, IMapper mapper, ILogger<ILogger> logger, IDistributedLockFactory distributedLockFactory,
            IOptions<CommentAddLockOptions> addLockOptionsAccessor)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _distributedLockFactory = distributedLockFactory;
            _addLockOptions = addLockOptionsAccessor.Value;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<CommentDetails>> GetAllAsync(int offset, int limit, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение списка комментариев с параметрами {2}: {3}, {4}: {5}",
                nameof(CommentRepository), nameof(GetAllAsync), nameof(offset), offset, nameof(limit), limit);

            var comments = await _repository.GetAll()
                .Where(c => c.IsActive == true)
                .ProjectTo<CommentDetails>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellation);

            return comments;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<CommentDetails>> GetAllFilteredAsync(CommentFilterRequest filterRequest, int offset, int limit, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение списка комментариев по фильтру c параметрами {2}: {3}, {4}: {5}, {6}: {7}.",
                nameof(CommentRepository), nameof(GetAllFilteredAsync), nameof(offset), offset, nameof(limit), limit, nameof(CategoryFilterRequest), 
                JsonConvert.SerializeObject(filterRequest));

            var query = _repository.GetAll()
                .Where(c => c.IsActive == true);

            if (filterRequest.UserReceiverId.HasValue)
            {
                query = query.Include(a => a.Advert)
                    .Where(a => a.Advert.UserId == filterRequest.UserReceiverId.Value);
            }
            if (filterRequest.UserAuthorId.HasValue)
            {
                query = query.Where(a => a.UserAuthorId == filterRequest.UserAuthorId);
            }
            if (filterRequest.AdvertId.HasValue)
            {
                query = query.Where(a => a.AdvertId == filterRequest.AdvertId);
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

            var comments = await query
                .ProjectTo<CommentDetails>(_mapper.ConfigurationProvider)
                .Skip(offset)
                .Take(limit)
                .ToListAsync(cancellation);

            return comments;
        }

        /// <inheritdoc />
        public async Task<CommentDetails> GetByIdAsync(Guid commentId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение комментария с ID: {2}",
                nameof(CommentRepository), nameof(GetByIdAsync), commentId);

            var comment = await _repository.GetAll()
                .Where(c => c.Id == commentId && c.IsActive == true)
                .ProjectTo<CommentDetails>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellation);

            return comment;
        }

        /// <inheritdoc />
        public async Task<float> GetUserRatingAsync(Guid userId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение среднего рейтинга комментариев для пользователя с ID: {2}",
                nameof(CommentRepository), nameof(GetUserRatingAsync), userId);

            var rating = await _repository.GetAll()
                .Include(c => c.Advert)
                .Where(c => c.Advert.UserId == userId && c.IsActive == true)
                .GroupBy(c => 1)
                .Select(c => (float)c.Count() != 0 ? (float)c.Sum(c => c.Rating) / (float)c.Count() : 0)
                .FirstOrDefaultAsync(cancellation);

            return rating;                
        }

        /// <inheritdoc />
        public async Task<Guid> GetUserIdAsync(Guid commentId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение ID пользователя, создавшего комментарий с ID: {2}",
                nameof(CommentRepository), nameof(GetUserIdAsync), commentId);

            var userId = await _repository.GetAll()
                .Where(c => c.Id == commentId && c.IsActive == true)
                .Select(c => c.UserAuthorId)
                .FirstOrDefaultAsync(cancellation);

            if(userId == Guid.Empty)
            {
                throw new KeyNotFoundException($"Не найден комментарий с ID: {commentId}");
            }

            return userId;
        }

        /// <inheritdoc />
        public async Task<Guid> AddIfNotExistsAsync(CommentAddRequest addRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Создание комментария из модели {2}: {3}",
                nameof(CommentRepository), nameof(AddIfNotExistsAsync), nameof(CategoryAddRequest), JsonConvert.SerializeObject(addRequest));

            var comment = _mapper.Map<CommentAddRequest, Comment>(addRequest);

            var resource = $"{_addLockOptions.CommentAddKey}_{addRequest.AdvertId}_{addRequest.UserAuthorId}";
            await using (var redLock = await _distributedLockFactory.CreateLockAsync(resource, _addLockOptions.Expire, _addLockOptions.Wait, _addLockOptions.Retry, cancellation))
            {
                if (redLock.IsAcquired)
                {
                    var isCommentExists = await _repository.GetAll()
                        .Where(c => c.UserAuthorId == addRequest.UserAuthorId)
                        .Where(c => c.AdvertId == addRequest.AdvertId)
                        .AnyAsync(cancellation);

                    if (isCommentExists)
                    {
                        throw new ArgumentException("Отзыв к этому обьявлению уже существует.");
                    }

                    await _repository.AddAsync(comment, cancellation);
                }
            }

            return comment.Id;
        }

        /// <inheritdoc />
        public async Task<CommentDetails> UpdateAsync(Guid commentId, CommentUpdateRequest updateRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Обновление комментария c ID: {2} из модели {3}: {4}",
                nameof(CommentRepository), nameof(UpdateAsync), commentId, nameof(CategoryUpdateRequest), JsonConvert.SerializeObject(updateRequest));

            var comment = await _repository.GetAll()
                 .Where(c => c.Id == commentId && c.IsActive == true)
                 .FirstOrDefaultAsync(cancellation);
            if (comment == null)
            {
                throw new KeyNotFoundException($"Не найден комментарий с ID: {commentId}");
            }

            var updatedComment = _mapper.Map<CommentUpdateRequest, Comment>(updateRequest, comment);
            await _repository.UpdateAsync(updatedComment, cancellation);
            var updatedCommentDto = _mapper.Map<Comment, CommentDetails>(updatedComment);

            return updatedCommentDto;
        }

        /// <inheritdoc />
        public async Task SoftDeleteAsync(Guid commentId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Мягкое удаление комментария с ID: {2}",
                nameof(CommentRepository), nameof(DeleteAsync), commentId);

            var comment = await _repository.GetByIdAsync(commentId, cancellation);
            if (comment == null)
            {
                throw new KeyNotFoundException($"Не найден комментарий с ID: {commentId}");
            }

            comment.IsActive = false;
            await _repository.UpdateAsync(comment, cancellation);
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Guid commentId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Удаление комментария с ID: {2}",
                nameof(CommentRepository), nameof(DeleteAsync), commentId);

            var comment = await _repository.GetByIdAsync(commentId, cancellation);
            if (comment == null)
            {
                throw new KeyNotFoundException($"Не найден комментарий с ID: {commentId}");
            }

            await _repository.DeleteAsync(comment, cancellation);
        }
    }
}
