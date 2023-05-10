using AutoMapper;
using AutoMapper.QueryableExtensions;
using Board.Application.AppData.Contexts.Comments.Repositories;
using Board.Contracts.Contexts.Adverts;
using Board.Contracts.Contexts.Categories;
using Board.Contracts.Contexts.Comments;
using Board.Domain;
using Board.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private const string CreateCommentKey = "CreateCommentKey_";

        public CommentRepository(IRepository<Comment> repository, IMapper mapper, ILogger<ILogger> logger, IDistributedLockFactory distributedLockFactory)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _distributedLockFactory = distributedLockFactory;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<CommentDetails>> GetAllAsync(int offset, int limit, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение всех комментариев.", nameof(GetAllAsync));

            var existingDtoList = await _repository.GetAll()
                .ProjectTo<CommentDetails>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellation);

            return existingDtoList;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<CommentDetails>> GetAllFilteredAsync(CommentFilterRequest filterRequest, int offset, int limit, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение всех комментариев по фильтру c параметрами {1}: {2}, {3}: {4}, {5}: {6}.",
                nameof(GetAllFilteredAsync), nameof(offset), offset, nameof(limit), limit, nameof(CategoryFilterRequest), JsonConvert.SerializeObject(filterRequest));

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
                .ProjectTo<CommentDetails>(_mapper.ConfigurationProvider)
                .Skip(offset)
                .Take(limit)
                .ToListAsync(cancellation);


            return existingDtoList;
        }

        /// <inheritdoc />
        public async Task<CommentDetails> GetByIdAsync(Guid commentId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение комментария с ID: {1}",
                nameof(GetByIdAsync), commentId);

            var existingDto = await _repository.GetAll()
                .Where(c => c.Id == commentId)
                .ProjectTo<CommentDetails>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellation);

            return existingDto;
        }

        /// <inheritdoc />
        public async Task<float> GetAverageRating(Guid userId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение среднего рейтинга комментариев для пользователя с ID: {1}",
                nameof(GetByIdAsync), userId);

            var rating = await _repository.GetAll()
                .Where(t => t.UserId == userId)
                .GroupBy(t => 1)
                .Select(c => new
                {
                     Rating = (float)c.Count() != 0 ? (float)c.Sum(c => c.Rating) / (float)c.Count() : 0
                }).FirstOrDefaultAsync(cancellation);

            if(rating == null)
            {
                return 0;
            }

            return rating.Rating;                
        }

        /// <inheritdoc />
        public async Task<Guid> AddAsync(CommentAddRequest addRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Создание комментария из модели {1}: {2}",
                nameof(AddAsync), nameof(CategoryAddRequest), JsonConvert.SerializeObject(addRequest));

            var newComment = _mapper.Map<CommentAddRequest, Comment>(addRequest);


            var resource = $"{CreateCommentKey}_{addRequest.AdvertisementId}_{addRequest.UserId}_{addRequest.AuthorId}";
            var expiry = TimeSpan.FromSeconds(30);
            var wait = TimeSpan.FromSeconds(10);
            var retry = TimeSpan.FromSeconds(1);
            await using (var redLock = await _distributedLockFactory.CreateLockAsync(resource, expiry, wait, retry, cancellation))
            {
                if (redLock.IsAcquired)
                {
                    var existingComment = await _repository.GetAll()
                        .Where(c => c.AuthorId == addRequest.AuthorId)
                        .Where(c => c.UserId == addRequest.UserId)
                        .Where(c => c.AdvertisementId == addRequest.AdvertisementId)
                        .FirstOrDefaultAsync(cancellation);

                    if (existingComment != null)
                    {
                        throw new ArgumentException("Отзыв к этому обьявлению уже существует.");
                    }

                    await _repository.AddAsync(newComment, cancellation);
                }
            }

            return newComment.Id;
        }

        /// <inheritdoc />
        public async Task<CommentDetails> UpdateAsync(Guid commentId, CommentUpdateRequest updateRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Обновление комментария c ID: {1} из модели {2}: {3}",
                nameof(UpdateAsync), commentId, nameof(CategoryUpdateRequest), JsonConvert.SerializeObject(updateRequest));

            var existingEntity = await _repository.GetByIdAsync(commentId, cancellation);
            if (existingEntity == null)
            {
                throw new KeyNotFoundException($"Не найден комментарий с ID: {commentId}");
            }

            var updatedEntity = _mapper.Map<CommentUpdateRequest, Comment>(updateRequest, existingEntity);
            await _repository.UpdateAsync(updatedEntity, cancellation);
            var updatedDto = _mapper.Map<Comment, CommentDetails>(updatedEntity);

            return updatedDto;
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Guid commentId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Удаление комментария с ID: {1}",
                nameof(DeleteAsync), commentId);

            var existingEntity = await _repository.GetByIdAsync(commentId, cancellation);
            if (existingEntity == null)
            {
                throw new KeyNotFoundException($"Не найден комментарий с ID: {commentId}");
            }

            await _repository.DeleteAsync(existingEntity, cancellation);
        }
    }
}
