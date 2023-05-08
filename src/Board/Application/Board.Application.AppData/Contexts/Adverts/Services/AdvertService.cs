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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using Board.Contracts.Exceptions;

namespace Board.Application.AppData.Contexts.Adverts.Services
{
    /// <inheritdoc />
    public class AdvertService : IAdvertService
    {
        private readonly IAdvertRepository _advertRepository;
        private readonly IAdvertImageRepository _advertImageRepository;
        private readonly IUserService _userService;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;
        private readonly IValidator<AdvertAddRequest> _advertAddValidator;
        private readonly IValidator<AdvertUpdateRequest> _advertUpdateValidator;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AdvertService> _logger;    
        private const int AdvertListCount = 10;


        public AdvertService(IAdvertRepository advertRepository, IMapper mapper, IValidator<AdvertAddRequest> advertAddValidator,
            IValidator<AdvertUpdateRequest> advertUpdateValidator, IAdvertImageRepository advertImageRepository, IUserService userService, IFileService fileService,
            IConfiguration configuration, ILogger<AdvertService> logger)
        {
            _advertRepository = advertRepository;
            _mapper = mapper;
            _advertAddValidator = advertAddValidator;
            _advertUpdateValidator = advertUpdateValidator;
            _advertImageRepository = advertImageRepository;
            _userService = userService;
            _fileService = fileService;
            _configuration = configuration;
            _logger = logger;
        }

        /// <inheritdoc />
        public Task<IReadOnlyCollection<AdvertSummary>> GetAllAsync(int? offset, int? count, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение всех обьявлений с параметрами: {1} = {2}, {3} = {4}", 
                nameof(GetAllAsync), nameof(offset), offset.GetValueOrDefault(), nameof(count), count.GetValueOrDefault());

            if (count.HasValue)
            {
                return _advertRepository.GetAllAsync(offset.GetValueOrDefault(), count.GetValueOrDefault(), cancellation);
            }

            try
            {
                count = Int32.Parse(_configuration.GetSection("Adverts").GetRequiredSection("ListDefaultCount").Value);
            }
            catch
            {
                _logger.LogWarning("{0} -> В конфигурации указано невалидное значение количества получаемых записей по умолчанию Adverts->ListDefaultCount",
                    nameof(GetAllAsync));
                count = AdvertListCount;
            }

            return _advertRepository.GetAllAsync(offset.GetValueOrDefault(), count.GetValueOrDefault(), cancellation);
        }

        /// <inheritdoc />
        public Task<IReadOnlyCollection<AdvertSummary>> GetAllFilteredAsync(AdvertFilterRequest request, int? offset, int? count, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение всех обьявлений по фильтру с параметрами: {1} = {2}, {3} = {4}, {5} = {6}",
                nameof(GetAllFilteredAsync), nameof(offset), offset.GetValueOrDefault(), nameof(count), count.GetValueOrDefault(), nameof(AdvertFilterRequest), 
                JsonConvert.SerializeObject(request));

            if (count.HasValue)
            {
                return _advertRepository.GetAllFilteredAsync(request, offset.GetValueOrDefault(), count.GetValueOrDefault(), cancellation);
            }

            try
            {
                count = Int32.Parse(_configuration.GetSection("Adverts").GetRequiredSection("ListDefaultCount").Value);
            }
            catch
            {
                _logger.LogWarning("{0} -> В конфигурации указано невалидное значение количества получаемых записей по умолчанию Adverts->ListDefaultCount",
                    nameof(GetAllAsync));
                count = AdvertListCount;
            }


            return _advertRepository.GetAllFilteredAsync(request, offset.GetValueOrDefault(), count.GetValueOrDefault(), cancellation);
        }

        /// <inheritdoc />
        public async Task<AdvertDetails> GetByIdAsync(Guid id, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Получение обьявления по ID: {1} ",
                nameof(GetByIdAsync), id);

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
            _logger.LogInformation("{0} -> Создание обьявления из модели {1}: {2}",
                nameof(CreateAsync), nameof(AdvertAddRequest), JsonConvert.SerializeObject(addRequest));

            var validationResult = _advertAddValidator.Validate(addRequest);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException($"Модель создания обьявления не прошла валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
            }

            var currentUser = await _userService.GetCurrentAsync(cancellation);
            if(currentUser == null)
            {
                throw new UnauthorizedAccessException($"При создании обьявления пользователь должен быть авторизован.");
            }

            addRequest.UserId = currentUser.Id.GetValueOrDefault();

            foreach (var imageId in addRequest.AdvertImagesId)
            {
                var fileInfo = _fileService.GetInfoAsync(imageId, cancellation);
                if(fileInfo == null)
                {
                    throw new KeyNotFoundException($"На файловом сервисе не найдено изображение с ID: {imageId}, указанное в модели создания обьявления.");
                }
            }

            var newAdvertId = await _advertRepository.AddAsync(addRequest, cancellation);

            foreach(var imageId in addRequest.AdvertImagesId)
            {
                var imageAddRequest = new AdvertImageAddRequest { AdvertId = newAdvertId, FileId = imageId };
                await _advertImageRepository.AddAsync(imageAddRequest, cancellation);
            }
            
            // TODO: если обьявление не добавилось, то удалить картинки?

            return newAdvertId;
        }

        /// <inheritdoc />
        public async Task<AdvertDetails> UpdateAsync(Guid id, AdvertUpdateRequest updateRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Обновление обьявления c ID: {1} из модели {2}: {3}",
                nameof(UpdateAsync), id, nameof(AdvertUpdateRequest), JsonConvert.SerializeObject(updateRequest));

            var validationResult = _advertUpdateValidator.Validate(updateRequest);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException($"Модель обновления обьявления не прошла валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
            }


            var currentAdvert = await _advertRepository.GetByIdAsync(id, cancellation);
            var currentUser = await _userService.GetCurrentAsync(cancellation);
            if (currentUser.Id != currentAdvert.UserId)
            {
                throw new ForbiddenException($"Обновить обьявление может только его владелец. Текущий пользователь имеет ID: {currentUser.Id}");
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
            updatedDto.User = currentUser;

            return updatedDto;
        }

        /// <inheritdoc />
        public async Task SoftDeleteAsync(Guid id, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Мягкое удаление обьявления с ID: {1}",
                nameof(SoftDeleteAsync), id);

            var currentAdvert = await _advertRepository.GetByIdAsync(id, cancellation);
            var currentUser = await _userService.GetCurrentAsync(cancellation);
            if (currentUser.Id != currentAdvert.UserId)
            {
                throw new ForbiddenException($"Удалить обьявление может только владелец. Текущий пользователь имеет ID: {currentUser.Id}");
            }

            await _advertRepository.SoftDeleteAsync(id, cancellation);
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Guid id, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Удаление обьявления с ID: {1}",
                nameof(DeleteAsync), id);

            await _advertRepository.DeleteAsync(id, cancellation);
        }
    }
}
