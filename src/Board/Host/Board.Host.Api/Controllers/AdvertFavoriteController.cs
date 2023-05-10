using Board.Application.AppData.Contexts.AdvertFavorites.Services;
using Board.Contracts.Contexts.AdvertFavorites;
using Board.Contracts.Contexts.Adverts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Board.Host.Api.Controllers
{
    public class AdvertFavoriteController : ControllerBase
    {
        private readonly IAdvertFavoriteService _advertFavoriteService;

        public AdvertFavoriteController(IAdvertFavoriteService advertFavoriteService)
        {
            _advertFavoriteService = advertFavoriteService;
        }

        [HttpGet("{userId:Guid}")]
        [AllowAnonymous]
        public async Task<IReadOnlyCollection<AdvertFavoriteSummary>> GetAllByUserId(Guid userId, CancellationToken cancellation)
        {
            var favoriteList = await _advertFavoriteService.GetAllByUserIdAsync(userId, cancellation);

            return (IReadOnlyCollection<AdvertFavoriteSummary>)Ok(favoriteList);
        }

        [HttpDelete("{favoriteId:Guid}")]
        public async Task<IActionResult> Delete(Guid favoriteId, CancellationToken cancellation)
        {
            await _advertFavoriteService.DeleteAsync(favoriteId, cancellation);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Guid advertId, Guid userId, CancellationToken cancellation)
        {
            var favoriteId = await _advertFavoriteService.AddAsync(advertId, userId, cancellation);

            return Ok(favoriteId);
        }
    }
}
