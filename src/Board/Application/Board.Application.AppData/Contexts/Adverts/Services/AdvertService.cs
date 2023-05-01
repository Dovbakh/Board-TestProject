using AutoMapper;
using Board.Application.AppData.Contexts.AdvertImages.Repositories;
using Board.Application.AppData.Contexts.Adverts.Repositories;
using Board.Application.AppData.Contexts.Adverts.Services;
using Board.Application.AppData.Contexts.Files.Services;
using Board.Application.AppData.Contexts.Users.Services;
using Board.Contracts.Contexts.AdvertImages;
using Board.Contracts.Contexts.Adverts;
using Board.Domain;
using FluentValidation;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using RedLockNet.SERedis.Configuration;
using RedLockNet.SERedis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Adverts.Services
{
    public class AdvertService : IAdvertService
    {
        private readonly IAdvertRepository _advertRepository;
        private readonly IAdvertImageRepository _advertImageRepository;
        private readonly IUserService _userService;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;
        private readonly IValidator<AdvertAddRequest> _advertAddValidator;
        private readonly IValidator<AdvertUpdateRequest> _advertUpdateValidator;
        private const string AdvertCountKey = "AdvertCountKey_";

        public AdvertService(IAdvertRepository advertRepository, IMapper mapper, IValidator<AdvertAddRequest> advertAddValidator,
            IValidator<AdvertUpdateRequest> advertUpdateValidator, IAdvertImageRepository advertImageRepository, IUserService userService, IFileService fileService)
        {
            _advertRepository = advertRepository;
            _mapper = mapper;
            _advertAddValidator = advertAddValidator;
            _advertUpdateValidator = advertUpdateValidator;
            _advertImageRepository = advertImageRepository;
            _userService = userService;
            _fileService = fileService;
        }

        public Task<IReadOnlyCollection<AdvertSummary>> GetAllAsync(int? offset, int? count, CancellationToken cancellation)
        {
            if (count == null)
            {
                count = 10; // TODO: в конфиг
            }

            return _advertRepository.GetAllAsync(offset.GetValueOrDefault(), count.GetValueOrDefault(), cancellation);
        }

        public Task<IReadOnlyCollection<AdvertSummary>> GetAllFilteredAsync(AdvertFilterRequest request, int? offset, int? count, CancellationToken cancellation)
        {
            if (count == null)
            {
                count = 10;
            }

            return _advertRepository.GetAllFilteredAsync(request, offset.GetValueOrDefault(), count.GetValueOrDefault(), cancellation);
        }

        public async Task<AdvertDetails> GetByIdAsync(Guid id, CancellationToken cancellation)
        {
            var advert = await _advertRepository.GetByIdAsync(id, cancellation);
            if (advert == null)
            {
                throw new KeyNotFoundException();
            }

            var user = await _userService.GetById(advert.UserId, cancellation);
            advert.User = user;

            return advert;

            // TODO: счетчик просмотров
            var resource = $"{AdvertCountKey}{id}";
            var expiry = TimeSpan.FromSeconds(30);
            var wait = TimeSpan.FromSeconds(10);
            var retry = TimeSpan.FromSeconds(1);

            var redlockFactory = RedLockFactory.Create(new List<RedLockEndPoint> { new DnsEndPoint("localhost", 6379) });
            // blocks until acquired or 'wait' timeout
            await using (var redLock = await redlockFactory.CreateLockAsync(resource, expiry, wait, retry, cancellation)) // there are also non async Create() methods
            {
                // make sure we got the lock
                if (redLock.IsAcquired)
                {
                    // do stuff
                    var a = 10;
                }
            }
        }


            public async Task<Guid> CreateAsync(AdvertAddRequest addRequest, CancellationToken cancellation)
        {
            // TODO: валидация
            var validationResult = _advertAddValidator.Validate(addRequest);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException(validationResult.ToString("~"));
            }

            // TODO: логирование


            var currentUser = await _userService.GetCurrent(cancellation);
            if(currentUser == null)
            {
                throw new KeyNotFoundException();
            }
            addRequest.UserId = currentUser.Id.GetValueOrDefault();

            // проверка наличия фото с указанными ID на сервере
            foreach (var imageId in addRequest.AdvertImagesId)
            {
                var fileInfo = _fileService.GetInfoAsync(imageId, cancellation);
                if(fileInfo == null)
                {
                    throw new KeyNotFoundException();
                }
            }

            var newAdvertId = await _advertRepository.AddAsync(addRequest, cancellation);

            foreach(var imageId in addRequest.AdvertImagesId)
            {
                var imageAddRequest = new AdvertImageAddRequest { AdvertId = newAdvertId, FileId = imageId };
                await _advertImageRepository.AddAsync(imageAddRequest, cancellation);
            }
            
            // TODO: если обьявление не добавилось, то удалить картинки

            return newAdvertId;
        }

        public Task<AdvertDetails> PatchAsync(Guid id, JsonPatchDocument<AdvertUpdateRequest> updateRequest, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public async Task<AdvertDetails> UpdateAsync(Guid id, AdvertUpdateRequest updateRequest, CancellationToken cancellation)
        {
            // TODO: валидация
            var validationResult = _advertUpdateValidator.Validate(updateRequest);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException(validationResult.ToString("~"));
            }

            // TODO: логирование
            var currentAdvert = await _advertRepository.GetByIdAsync(id, cancellation);
            var currentUser = await _userService.GetCurrent(cancellation);
            if (currentUser.Id != currentAdvert.UserId)
            {
                throw new UnauthorizedAccessException();
            }


            foreach (var imageId in updateRequest.NewAdvertImagesId)
            {
                var imageAddRequest = new AdvertImageAddRequest { AdvertId = id, FileId = imageId };
                await _advertImageRepository.AddAsync(imageAddRequest, cancellation);
            }

            foreach (var imageId in updateRequest.RemovedAdvertImagesId)
            {
                await _advertImageRepository.DeleteByFileIdAsync(imageId, cancellation);
            }

            var updatedDto = await _advertRepository.UpdateAsync(id, updateRequest, cancellation);



            var user = await _userService.GetById(updatedDto.UserId, cancellation);
            updatedDto.User = user;

            return updatedDto;
        }

        public async Task SoftDeleteAsync(Guid id, CancellationToken cancellation)
        {
            await _advertRepository.SoftDeleteAsync(id, cancellation);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellation)
        {
            var currentAdvert = await _advertRepository.GetByIdAsync(id, cancellation);
            var currentUser = await _userService.GetCurrent(cancellation);
            if (currentUser.Id != currentAdvert.UserId)
            {
                throw new UnauthorizedAccessException();
            }

            await _advertRepository.DeleteAsync(id, cancellation);
        }
    }
}
