using Board.Application.AppData.Contexts.Adverts.Repositories;
using Board.Application.AppData.Contexts.Adverts.Services;
using Board.Application.AppData.Contexts.Comments.Repositories;
using Board.Application.AppData.Contexts.Users.Services;
using Board.Contracts.Contexts.Adverts;
using Board.Contracts.Contexts.Categories;
using Board.Contracts.Contexts.Comments;
using Board.Contracts.Exceptions;
using Board.Contracts.Options;
using Board.Domain;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Comments.Services
{
    /// <inheritdoc />
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IUserService _userService;
        private readonly IAdvertService _advertService;
        private readonly IAdvertRepository _advertRepository;
        private readonly IValidator<CommentAddRequest> _commentAddValidator;
        private readonly IValidator<CommentUpdateRequest> _commentUpdateValidator;
        private readonly IDistributedLockFactory _distributedLockFactory;
        private readonly ILogger<CommentService> _logger;
        private readonly CommentOptions _commentOptions;

        public CommentService(ICommentRepository commentRepository, IUserService userService, IAdvertService advertService, IAdvertRepository advertRepository,
            IValidator<CommentAddRequest> commentAddValidator, IValidator<CommentUpdateRequest> commentUpdateValidator, IDistributedLockFactory distributedLockFactory,
            ILogger<CommentService> logger, IOptions<CommentOptions> commentOptionsAccessor)
        {
            _commentRepository = commentRepository;
            _userService = userService;
            _advertService = advertService;
            _advertRepository = advertRepository;
            _commentAddValidator = commentAddValidator;
            _commentUpdateValidator = commentUpdateValidator;
            _distributedLockFactory = distributedLockFactory;
            _logger = logger;
            _commentOptions = commentOptionsAccessor.Value;
        }


        /// <inheritdoc />
        public async Task<IReadOnlyCollection<CommentDetails>> GetAllAsync(int? offset, int? count, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение всех отзывов c параметрами {2}: {3}, {4}: {5}", 
                nameof(CommentService), nameof(GetAllAsync), nameof(offset), offset, nameof(count), count);

            if (!count.HasValue)
            {
                count = _commentOptions.ListDefaultCount;
            }

            var comments = await _commentRepository.GetAllAsync(offset.GetValueOrDefault(), count.GetValueOrDefault(), cancellation);

            return comments;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<CommentDetails>> GetAllFilteredAsync(CommentFilterRequest filterRequest, int? offset, int? count, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение всех отзывов по фильтру c параметрами {2}: {3}, {4}: {5}, {6}: {7}.",
                nameof(CommentService), nameof(GetAllFilteredAsync), nameof(offset), offset, nameof(count), count, nameof(CommentFilterRequest), 
                JsonConvert.SerializeObject(filterRequest));

            if (!count.HasValue)
            {
                count = _commentOptions.ListDefaultCount;
            }

            var comments =  await _commentRepository.GetAllFilteredAsync(filterRequest, offset.GetValueOrDefault(), count.GetValueOrDefault(), cancellation);

            return comments;
        }

        /// <inheritdoc />
        public async Task<CommentDetails> GetByIdAsync(Guid commentId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение отзыва с ID: {2}",
                nameof(CommentService), nameof(GetByIdAsync), commentId);

            var comment = await _commentRepository.GetByIdAsync(commentId, cancellation);
            if (comment == null)
            {
                throw new KeyNotFoundException($"Не найден отзыв с ID: {commentId} ");
            }

            return comment;
        }

        /// <inheritdoc />
        public async Task<Guid> CreateAsync(CommentAddRequest addRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Создание отзыва из модели {2}: {3}",
                nameof(CommentService), nameof(CreateAsync), nameof(CategoryAddRequest), JsonConvert.SerializeObject(addRequest));

            await _commentAddValidator.ValidateAndThrowAsync(addRequest, cancellation);


            addRequest.UserAuthorId = _userService.GetCurrentId(cancellation).Value;

            var advert = await _advertRepository.GetByIdAsync(addRequest.AdvertId, cancellation);
            if (advert == null)
            {
                throw new KeyNotFoundException($"Обьявления {addRequest.AdvertId} не существует.");
            }

            if (advert.UserId == addRequest.UserAuthorId)
            {
                throw new ArgumentException("Нельзя оставить отзыв самому себе.");
            }

            var commentId = await _commentRepository.AddIfNotExistsAsync(addRequest, cancellation);

            return commentId;
        }

        /// <inheritdoc />
        public async Task<CommentDetails> UpdateAsync(Guid commentId, CommentUpdateRequest updateRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Обновление отзыва c ID: {2} из модели {3}: {4}",
                nameof(CommentService), nameof(UpdateAsync), commentId, nameof(CategoryUpdateRequest), JsonConvert.SerializeObject(updateRequest));

            await _commentUpdateValidator.ValidateAndThrowAsync(updateRequest, cancellation);


            var commentUserId = await _commentRepository.GetUserIdAsync(commentId, cancellation);
            var currentUserId = _userService.GetCurrentId(cancellation);
            if (commentUserId != currentUserId)
            {
                throw new ForbiddenException($"Нет доступа для обновления данного отзыва.");
            }

            var updatedComment = await _commentRepository.UpdateAsync(commentId, updateRequest, cancellation);

            return updatedComment;
        }

        /// <inheritdoc />
        public async Task SoftDeleteAsync(Guid commentId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Мягкое удаление отзыва с ID: {2}",
                nameof(CommentService), nameof(SoftDeleteAsync), commentId);

            var commentUserId = await _commentRepository.GetUserIdAsync(commentId, cancellation);
            var currentUserId = _userService.GetCurrentId(cancellation);
            if (commentUserId != currentUserId)
            {
                throw new ForbiddenException($"Нет доступа для удаления данного отзыва.");
            }

            await _commentRepository.SoftDeleteAsync(commentId, cancellation);
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Guid commentId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Удаление отзыва с ID: {2}",
                nameof(CommentService), nameof(SoftDeleteAsync), commentId);

            await _commentRepository.DeleteAsync(commentId, cancellation);
        }
    }
}
