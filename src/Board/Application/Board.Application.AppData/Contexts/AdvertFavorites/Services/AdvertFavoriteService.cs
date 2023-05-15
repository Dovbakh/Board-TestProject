﻿using Board.Application.AppData.Contexts.AdvertFavorites.Repositories;
using Board.Application.AppData.Contexts.Adverts.Repositories;
using Board.Application.AppData.Contexts.AdvertViews.Repositories;
using Board.Application.AppData.Contexts.Users.Services;
using Board.Contracts.Contexts.AdvertFavorites;
using Board.Contracts.Contexts.Adverts;
using Board.Contracts.Options;
using Board.Contracts.Exceptions;
using Board.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.AdvertFavorites.Services
{
    /// <inheritdoc />
    public class AdvertFavoriteService : IAdvertFavoriteService
    {
        private readonly IAdvertFavoriteRepository _advertFavoriteRepository;
        private readonly IAdvertRepository _advertRepository;
        private readonly IUserService _userService;
        private readonly ILogger<AdvertFavoriteService> _logger;
        

        public AdvertFavoriteService(IAdvertFavoriteRepository advertFavoriteRepository, ILogger<AdvertFavoriteService> logger, IAdvertRepository advertRepository,
            IUserService userService, IHttpContextAccessor contextAccessor)
        {
            _advertFavoriteRepository = advertFavoriteRepository;
            _logger = logger;
            _advertRepository = advertRepository;
            _userService = userService;
        }

        /// <inheritdoc />
        public async Task AddIfNotExistsAsync(Guid advertId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Добавление обьявления с ID: {2} в избранное пользователя.",
                nameof(AdvertFavoriteService), nameof(AddIfNotExistsAsync), advertId);


            var isAdvertExists = await _advertRepository.IsExists(advertId, cancellation);
            if (!isAdvertExists)
            {
                throw new KeyNotFoundException($"Не найдено обьявление с ID: {advertId}");
            }

            var isUserLogined = await _userService.IsLoginedAsync(cancellation);
            if(!isUserLogined)
            {
                _advertFavoriteRepository.AddToCookieIfNotExists(advertId, cancellation);
                return;
            }

            var currentUserId = _userService.GetCurrentId(cancellation).Value;
            await _advertFavoriteRepository.AddIfNotExistsAsync(advertId, currentUserId, cancellation);         
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Guid advertId, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Удаление из избранного обьявления с ID: {2}",
                nameof(AdvertFavoriteService), nameof(DeleteAsync), advertId);


            var isUserLogined = await _userService.IsLoginedAsync(cancellation);
            if (!isUserLogined)
            {
                _advertFavoriteRepository.DeleteFromCookie(advertId, cancellation);
                return;
            }

            var currentUserId = _userService.GetCurrentId(cancellation).Value;
            await _advertFavoriteRepository.DeleteAsync(advertId, currentUserId, cancellation);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<Guid>> GetAllForCurrentUserAsync(CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение списка избранных обьявлений для текущего пользователя.",
                nameof(AdvertFavoriteService), nameof(GetAllForCurrentUserAsync));

            var isUserLogined = await _userService.IsLoginedAsync(cancellation);
            if (!isUserLogined)
            {
                return _advertFavoriteRepository.GetAllFromCookie(cancellation);
            }

            var currentUserId = _userService.GetCurrentId(cancellation).Value;
            return await _advertFavoriteRepository.GetAllByUserIdAsync(currentUserId, cancellation);
        }        
    }
}
