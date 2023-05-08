using Board.Application.AppData.Contexts.Adverts.Repositories;
using Board.Application.AppData.Contexts.Adverts.Services;
using Board.Application.AppData.Contexts.Comments.Repositories;
using Board.Application.AppData.Contexts.Users.Services;
using Board.Contracts.Contexts.Categories;
using Board.Contracts.Contexts.Comments;
using Board.Domain;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly IConfiguration _configuration;
        private readonly ILogger<CommentService> _logger;
        private const int CommentListCount = 10;
        private const string CreateCommentKey = "CreateCommentKey_";

        public CommentService(ICommentRepository commentRepository, IUserService userService, IAdvertService advertService, IAdvertRepository advertRepository,
            IValidator<CommentAddRequest> commentAddValidator, IValidator<CommentUpdateRequest> commentUpdateValidator, IDistributedLockFactory distributedLockFactory,
            IConfiguration configuration, ILogger<CommentService> logger)
        {
            _commentRepository = commentRepository;
            _userService = userService;
            _advertService = advertService;
            _advertRepository = advertRepository;
            _commentAddValidator = commentAddValidator;
            _commentUpdateValidator = commentUpdateValidator;
            _distributedLockFactory = distributedLockFactory;
            _configuration = configuration;
            _logger = logger;
        }


        /// <inheritdoc />
        public Task<IReadOnlyCollection<CommentDetails>> GetAllAsync(int? offset, int? count, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение всех комментариев.", nameof(GetAllAsync));

            if (count.HasValue)
            {
                return _commentRepository.GetAllAsync(offset.GetValueOrDefault(), count.GetValueOrDefault(), cancellation);
            }

            try
            {
                count = Int32.Parse(_configuration.GetSection("Comments").GetRequiredSection("ListDefaultCount").Value);
            }
            catch
            {
                _logger.LogWarning("{0} -> В конфигурации указано невалидное значение количества получаемых записей по умолчанию Comments->ListDefaultCount",
                    nameof(GetAllAsync));
                count = CommentListCount;
            }

            return _commentRepository.GetAllAsync(offset.GetValueOrDefault(), count.GetValueOrDefault(), cancellation);
        }

        /// <inheritdoc />
        public Task<IReadOnlyCollection<CommentDetails>> GetAllFilteredAsync(CommentFilterRequest filterRequest, int? offset, int? count, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение всех комментариев по фильтру c параметрами {1}: {2}, {3}: {4}, {5}: {6}.",
                nameof(GetAllFilteredAsync), nameof(offset), offset, nameof(count), count, nameof(CommentFilterRequest), JsonConvert.SerializeObject(filterRequest));

            if (count.HasValue)
            {
                return _commentRepository.GetAllFilteredAsync(filterRequest, offset.GetValueOrDefault(), count.GetValueOrDefault(), cancellation);
            }

            try
            {
                count = Int32.Parse(_configuration.GetSection("Comments").GetRequiredSection("ListDefaultCount").Value);
            }
            catch
            {
                _logger.LogWarning("{0} -> В конфигурации указано невалидное значение количества получаемых записей по умолчанию Comments->ListDefaultCount",
                    nameof(GetAllAsync));
                count = CommentListCount;
            }

            return _commentRepository.GetAllFilteredAsync(filterRequest, offset.GetValueOrDefault(), count.GetValueOrDefault(), cancellation);
        }

        /// <inheritdoc />
        public Task<CommentDetails> GetByIdAsync(Guid id, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение комментария с ID: {1}",
                nameof(GetByIdAsync), id);

            var comment = _commentRepository.GetByIdAsync(id, cancellation);
            if (comment == null)
            {
                throw new KeyNotFoundException($"Не найден комментарий с ID: {id} ");
            }

            return comment;
        }

        /// <inheritdoc />
        public async Task<Guid> CreateAsync(CommentAddRequest addRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Создание комментария из модели {1}: {2}",
                nameof(CreateAsync), nameof(CategoryAddRequest), JsonConvert.SerializeObject(addRequest));

            var validationResult = _commentAddValidator.Validate(addRequest);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException($"Модель создания комментария не прошла валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
            }

            var author = await _userService.GetCurrentAsync(cancellation);
            addRequest.AuthorId = author.Id.GetValueOrDefault();

            var user = await _userService.GetByIdAsync(addRequest.UserId, cancellation);
            if (user == null)
            {
                throw new KeyNotFoundException($"Пользователя {addRequest.UserId} не существует.");
            }

            var advert = await _advertRepository.GetByIdAsync(addRequest.AdvertisementId, cancellation);
            if (advert == null)
            {
                throw new KeyNotFoundException($"Обьявления {addRequest.AdvertisementId} не существует.");
            }

            if (advert.UserId != user.Id)
            {
                throw new ArgumentException($"Обьявление {advert.Id} не относится к пользователю {user.Id}");
            }

            if (advert.UserId == author.Id)
            {
                throw new ArgumentException("Нельзя оставить отзыв самому себе.");
            }

            var commentId = await _commentRepository.AddAsync(addRequest, cancellation);

            return commentId;
        }

        /// <inheritdoc />
        public Task<CommentDetails> UpdateAsync(Guid id, CommentUpdateRequest updateRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Обновление комментария c ID: {1} из модели {2}: {3}",
                nameof(UpdateAsync), id, nameof(CategoryUpdateRequest), JsonConvert.SerializeObject(updateRequest));

            var validationResult = _commentUpdateValidator.Validate(updateRequest);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException(validationResult.ToString("~"));
            }

            return _commentRepository.UpdateAsync(id, updateRequest, cancellation);
        }

        /// <inheritdoc />
        public Task DeleteAsync(Guid id, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Удаление комментария с ID: {1}",
                nameof(DeleteAsync), id);

            return _commentRepository.DeleteAsync(id, cancellation);
        }
    }
}
