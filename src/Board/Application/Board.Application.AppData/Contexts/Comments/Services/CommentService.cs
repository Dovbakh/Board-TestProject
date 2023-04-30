using Board.Application.AppData.Contexts.Adverts.Repositories;
using Board.Application.AppData.Contexts.Adverts.Services;
using Board.Application.AppData.Contexts.Comments.Repositories;
using Board.Application.AppData.Contexts.Users.Services;
using Board.Contracts.Contexts.Comments;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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

        public CommentService(ICommentRepository commentRepository, IUserService userService, IAdvertService advertService, IAdvertRepository advertRepository, 
            IValidator<CommentAddRequest> commentAddValidator, IValidator<CommentUpdateRequest> commentUpdateValidator)
        {
            _commentRepository = commentRepository;
            _userService = userService;
            _advertService = advertService;
            _advertRepository = advertRepository;
            _commentAddValidator = commentAddValidator;
            _commentUpdateValidator = commentUpdateValidator;
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
                throw new KeyNotFoundException();
            }

            var advert = await _advertRepository.GetByIdAsync(addRequest.AdvertisementId, cancellation);              
            if(advert == null)
            {
                throw new KeyNotFoundException();
            }

            if (advert.UserId != user.Id)
            {
                throw new ArgumentException();
            }

            if(advert.UserId == author.Id)
            {
                throw new ArgumentException();
            }

            return await _commentRepository.AddAsync(addRequest, cancellation);
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
