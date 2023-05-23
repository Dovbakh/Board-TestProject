using Board.Application.AppData.Contexts.AdvertFavorites.Services;
using Board.Contracts.Contexts.AdvertFavorites;
using Board.Contracts.Contexts.Adverts;
using Board.Contracts.Conventions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Board.Host.Api.Controllers
{
    /// <summary>
    /// Контроллер для работы с избранным обьявлений.
    /// </summary>
    [ApiController]
    [Route("v2/[controller]")]
    [Produces("application/json")]
    [ApiConventionType(typeof(AppConventions))]
    public class AdvertFavoritesController : ControllerBase
    {
        private readonly IAdvertFavoriteService _advertFavoriteService;

        public AdvertFavoritesController(IAdvertFavoriteService advertFavoriteService)
        {
            _advertFavoriteService = advertFavoriteService;
        }

        /// <summary>
        /// Получить обьявления из избранного текущего пользователя. [anonymous]
        /// </summary>
        /// <param name="offset">Количество пропускаемых обьявлений.</param>
        /// <param name="limit">Количество получаемых обьявлений.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Спиисок обьявлений.</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyCollection<AdvertSummary>>> GetAdvertsForCurrentUserId(int? offset, int? limit, CancellationToken cancellation)
        {
            var advertFavorites = await _advertFavoriteService.GetAdvertsForCurrentUserAsync(offset, limit, cancellation);

            return Ok(advertFavorites);
        }

        /// <summary>
        /// Удалить обьявление из избранного текущего пользователя. [anonymous]
        /// </summary>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns></returns>
        [HttpDelete("{advertId:Guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteForCurrentUser(Guid advertId, CancellationToken cancellation)
        {
            await _advertFavoriteService.DeleteAsync(advertId, cancellation);

            return NoContent();
        }

        /// <summary>
        /// Добавить обьявление в избранное текущего пользователя. [anonymous]
        /// </summary>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddForCurrentUser([FromBody]Guid advertId, CancellationToken cancellation)
        {
            await _advertFavoriteService.AddIfNotExistsAsync(advertId, cancellation);

            return Ok();
        }

        /// <summary>
        /// Получить идентификаторы обьявлений из избранного текущего пользователя. [anonymous]
        /// </summary>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Список идентификаторов.</returns>
        [HttpGet("ids")]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyCollection<Guid>>> GetIdsForCurrentUserId(CancellationToken cancellation)
        {
            var advertFavoriteIds = await _advertFavoriteService.GetIdsForCurrentUserAsync(cancellation);

            return Ok(advertFavoriteIds);
        }
    }
}
