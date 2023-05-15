using AutoMapper;
using Board.Application.AppData.Contexts.AdvertImages.Repositories;
using Board.Application.AppData.Contexts.Adverts.Repositories;
using Board.Application.AppData.Contexts.Adverts.Services;
using Board.Application.AppData.Contexts.Images.Services;
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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using Board.Contracts.Exceptions;
using Board.Application.AppData.Contexts.AdvertViews.Services;
using Microsoft.Extensions.Options;
using Board.Contracts.Options;

namespace Board.Application.AppData.Contexts.Adverts.Services
{
    /// <inheritdoc />
    public class AdvertService : IAdvertService
    {
        private readonly IAdvertRepository _advertRepository;
        private readonly IAdvertImageRepository _advertImageRepository;
        private readonly IAdvertViewService _advertViewService;
        private readonly IUserService _userService;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;
        private readonly IValidator<AdvertAddRequest> _advertAddValidator;
        private readonly IValidator<AdvertUpdateRequest> _advertUpdateValidator;
        private readonly ILogger<AdvertService> _logger;   
        private readonly AdvertOptions _advertOptions;


        public AdvertService(IAdvertRepository advertRepository, IMapper mapper, IValidator<AdvertAddRequest> advertAddValidator,
            IValidator<AdvertUpdateRequest> advertUpdateValidator, IAdvertImageRepository advertImageRepository, IUserService userService, IImageService imageService,
            ILogger<AdvertService> logger, IAdvertViewService advertViewService, IOptions<AdvertOptions> advertOptionsAccessor)
        {
            _advertRepository = advertRepository;
            _mapper = mapper;
            _advertAddValidator = advertAddValidator;
            _advertUpdateValidator = advertUpdateValidator;
            _advertImageRepository = advertImageRepository;
            _userService = userService;
            _imageService = imageService;
            _logger = logger;
            _advertViewService = advertViewService;
            _advertOptions = advertOptionsAccessor.Value;
        }

        /// <inheritdoc />
        public Task<IReadOnlyCollection<AdvertSummary>> GetAllAsync(int? offset, int? count, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение списка обьявлений с параметрами: {2} = {3}, {4} = {5}", 
                nameof(AdvertService), nameof(GetAllAsync), nameof(offset), offset, nameof(count), count);

            if (!count.HasValue)
            {
                count = _advertOptions.ListDefaultCount;
            }

            return _advertRepository.GetAllAsync(offset.GetValueOrDefault(), count.GetValueOrDefault(), cancellation);
        }

        /// <inheritdoc />
        public Task<IReadOnlyCollection<AdvertSummary>> GetAllFilteredAsync(AdvertFilterRequest request, int? offset, int? count, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение всех обьявлений по фильтру с параметрами: {2} = {3}, {4} = {5}, {6} = {7}",
                nameof(AdvertService), nameof(GetAllFilteredAsync), nameof(offset), offset, nameof(count), count, nameof(AdvertFilterRequest), 
                JsonConvert.SerializeObject(request));

            if (!count.HasValue)
            {
                count = _advertOptions.ListDefaultCount;
            }

            return _advertRepository.GetAllFilteredAsync(request, offset.GetValueOrDefault(), count.GetValueOrDefault(), cancellation);
        }

        /// <inheritdoc />
        public async Task<AdvertDetails> GetByIdAsync(Guid id, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение обьявления по ID: {2} ",
                nameof(AdvertService), nameof(GetByIdAsync), id);

            var advert = await _advertRepository.GetByIdAsync(id, cancellation);
            if (advert == null)
            {
                throw new KeyNotFoundException($"Не найдено обьявление с ID: {id}");
            }

            var user = await _userService.GetByIdAsync(advert.UserId, cancellation);
            if (user == null)
            {
                throw new KeyNotFoundException($"Не найден пользователь с ID: {advert.UserId}, указанный в обьявлении с ID: {id}"); ;
            }
            advert.User = user;

            return advert;           
        }

        /// <inheritdoc />
        public async Task<Guid> CreateAsync(AdvertAddRequest addRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Создание обьявления из модели {2}: {3}",
                nameof(AdvertService), nameof(CreateAsync), nameof(AdvertAddRequest), JsonConvert.SerializeObject(addRequest));

            var validationResult = _advertAddValidator.Validate(addRequest);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException($"Модель создания обьявления не прошла валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
            }

            var currentUserId = _userService.GetCurrentId(cancellation);
            if(!currentUserId.HasValue)
            {
                throw new UnauthorizedAccessException($"При создании обьявления пользователь должен быть авторизован.");
            }
            addRequest.UserId = currentUserId.Value;

            foreach (var imageId in addRequest.ImagesId)
            {
                var isImageExists = await _imageService.IsImageExists(imageId, cancellation);
                if(!isImageExists)
                {
                    throw new KeyNotFoundException($"На файловом сервисе не найдено изображение с ID: {imageId}, указанное в модели создания обьявления.");
                }
            }

            var newAdvertId = await _advertRepository.AddAsync(addRequest, cancellation);

            foreach(var imageId in addRequest.ImagesId)
            {
                var imageAddRequest = new AdvertImageAddRequest { AdvertId = newAdvertId, ImageId = imageId };
                await _advertImageRepository.AddAsync(imageAddRequest, cancellation);
            }        

            return newAdvertId;
        }

        /// <inheritdoc />
        public async Task<AdvertDetails> UpdateAsync(Guid id, AdvertUpdateRequest updateRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Обновление обьявления c ID: {2} из модели {3}: {4}",
                nameof(AdvertService), nameof(UpdateAsync), id, nameof(AdvertUpdateRequest), JsonConvert.SerializeObject(updateRequest));

            var validationResult = _advertUpdateValidator.Validate(updateRequest);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException($"Модель обновления обьявления не прошла валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
            }

            var advertUserId = await _advertRepository.GetUserIdAsync(id, cancellation);
            var hasPermission = _userService.HasPermission(advertUserId, cancellation);
            if (!hasPermission)
            {
                throw new ForbiddenException($"Нет доступа для обновления текущего обьявления.");
            }

            foreach (var imageId in updateRequest.NewImagesId)
            {
                var imageAddRequest = new AdvertImageAddRequest { AdvertId = id, ImageId = imageId };
                await _advertImageRepository.AddAsync(imageAddRequest, cancellation);
            }
            foreach (var imageId in updateRequest.RemovedImagesId)
            {
                await _advertImageRepository.DeleteByFileIdAsync(imageId, cancellation);
            }

            var updatedAdvert = await _advertRepository.UpdateAsync(id, updateRequest, cancellation);

            var user = await _userService.GetByIdAsync(updatedAdvert.UserId, cancellation);
            if (user == null)
            {
                throw new KeyNotFoundException($"Не найден пользователь с ID: {updatedAdvert.UserId}, указанный в обьявлении с ID: {id}"); ;
            }
            updatedAdvert.User = user;

            return updatedAdvert;
        }

        /// <inheritdoc />
        public async Task SoftDeleteAsync(Guid id, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Мягкое удаление обьявления с ID: {2}",
                nameof(AdvertService), nameof(SoftDeleteAsync), id);

            var advertUserId = await _advertRepository.GetUserIdAsync(id, cancellation);
            var hasPermission = _userService.HasPermission(advertUserId, cancellation);
            if(!hasPermission)
            {
                throw new ForbiddenException($"Нет доступа для удаления текущего обьявления.");
            }

            await _advertRepository.SoftDeleteAsync(id, cancellation);
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Guid id, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Удаление обьявления с ID: {2}",
                nameof(AdvertService), nameof(DeleteAsync), id);

            await _advertRepository.DeleteAsync(id, cancellation);
        }
    }
}
