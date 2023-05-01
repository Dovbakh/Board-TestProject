using Board.Application.AppData.Contexts.Adverts.Repositories;
using Board.Application.AppData.Contexts.Adverts.Services;
using Board.Application.AppData.Contexts.Comments.Repositories;
using Board.Application.AppData.Contexts.Users.Services;
using Board.Contracts.Contexts.Comments;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Comments.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IUserService _userService;
        private readonly IAdvertService _advertService;
        private readonly IAdvertRepository _advertRepository;
        private readonly IValidator<CommentAddRequest> _commentAddValidator;
        private readonly IValidator<CommentUpdateRequest> _commentUpdateValidator;
        private readonly IDistributedLockFactory _distributedLockFactory;

        private const string CreateCommentKey = "CreateCommentKey_";

        public CommentService(ICommentRepository commentRepository, IUserService userService, IAdvertService advertService, IAdvertRepository advertRepository,
            IValidator<CommentAddRequest> commentAddValidator, IValidator<CommentUpdateRequest> commentUpdateValidator, IDistributedLockFactory distributedLockFactory)
        {
            _commentRepository = commentRepository;
            _userService = userService;
            _advertService = advertService;
            _advertRepository = advertRepository;
            _commentAddValidator = commentAddValidator;
            _commentUpdateValidator = commentUpdateValidator;
            _distributedLockFactory = distributedLockFactory;
        }

        public async Task<Guid> CreateAsync(CommentAddRequest addRequest, CancellationToken cancellation)
        {
            var validationResult = _commentAddValidator.Validate(addRequest);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException(validationResult.ToString("~"));
            }


            var author = await _userService.GetCurrent(cancellation);
            addRequest.AuthorId = author.Id.GetValueOrDefault();

            var user = await _userService.GetById(addRequest.UserId, cancellation);
            if(user == null) 
            {
                throw new KeyNotFoundException($"Пользователя {addRequest.UserId} не существует");
            }

            var advert = await _advertRepository.GetByIdAsync(addRequest.AdvertisementId, cancellation);              
            if(advert == null)
            {
                throw new KeyNotFoundException($"Обьявления {addRequest.AdvertisementId} не существует");
            }

            if (advert.UserId != user.Id)
            {
                throw new ArgumentException($"Обьявление {advert.Id} не относится к пользователю {user.Id}");
            }

            if(advert.UserId == author.Id)
            {
                throw new ArgumentException("Нельзя оставить отзыв самому себе.");
            }

            var commentId = Guid.Empty;

            var resource = $"{CreateCommentKey}_{addRequest.AdvertisementId}_{addRequest.UserId}_{addRequest.AuthorId}";
            var expiry = TimeSpan.FromSeconds(30);
            var wait = TimeSpan.FromSeconds(10);
            var retry = TimeSpan.FromSeconds(1);
            await using (var redLock = await _distributedLockFactory.CreateLockAsync(resource, expiry, wait, retry, cancellation))
            {
                if (redLock.IsAcquired)
                {
                    var filterRequest = new CommentFilterRequest { AuthorId = author.Id, UserId = user.Id, AdvertisementId = advert.Id };
                    var comment = await GetAllFilteredAsync(filterRequest, 0, 1, cancellation);
                    if (comment.Count != 0)
                    {
                        throw new ArgumentException("Отзыв к этому обьявлению уже существует.");
                    }

                    commentId = await _commentRepository.AddAsync(addRequest, cancellation);
                }
            }

            if(commentId == Guid.Empty) 
            {
                throw new ArgumentException();
            }
           
            return commentId;
        }

        public Task DeleteAsync(Guid id, CancellationToken cancellation)
        {
            return _commentRepository.DeleteAsync(id, cancellation);
        }

        public Task<IReadOnlyCollection<CommentDetails>> GetAllAsync(int? offset, int? count, CancellationToken cancellation)
        {
            if (count == 0)
            {
                count = 10;
            }
            return _commentRepository.GetAllAsync(offset.GetValueOrDefault(), count.GetValueOrDefault(), cancellation);
        }

        public Task<IReadOnlyCollection<CommentDetails>> GetAllFilteredAsync(CommentFilterRequest filterRequest, int? offset, int? count, CancellationToken cancellation)
        {
            if (count == 0)
            {
                count = 10;
            }
            return _commentRepository.GetAllFilteredAsync(filterRequest, offset.GetValueOrDefault(), count.GetValueOrDefault(), cancellation);
        }

        public Task<CommentDetails> GetByIdAsync(Guid id, CancellationToken cancellation)
        {
            return _commentRepository.GetByIdAsync(id, cancellation);
        }

        public Task<CommentDetails> PatchAsync(Guid id, JsonPatchDocument<CommentUpdateRequest> updateRequest, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task<CommentDetails> UpdateAsync(Guid id, CommentUpdateRequest updateRequest, CancellationToken cancellation)
        {
            var validationResult = _commentUpdateValidator.Validate(updateRequest);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException(validationResult.ToString("~"));
            }

            return _commentRepository.UpdateAsync(id, updateRequest, cancellation);
        }
    }
}
