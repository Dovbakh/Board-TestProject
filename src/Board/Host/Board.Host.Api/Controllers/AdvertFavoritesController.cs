using Board.Application.AppData.Contexts.AdvertFavorites.Services;
using Board.Contracts.Contexts.AdvertFavorites;
using Board.Contracts.Contexts.Adverts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Board.Host.Api.Controllers
{
    [ApiController]
    [Route("v2/[controller]")]
    [Produces("application/json")]
    public class AdvertFavoritesController : ControllerBase
    {
        private readonly IAdvertFavoriteService _advertFavoriteService;

        public AdvertFavoritesController(IAdvertFavoriteService advertFavoriteService)
        {
            _advertFavoriteService = advertFavoriteService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyCollection<AdvertSummary>>> GetAdvertsForCurrentUserId(int? offset, int? limit, CancellationToken cancellation)
        {
            var advertFavorites = await _advertFavoriteService.GetAdvertsForCurrentUserAsync(offset, limit, cancellation);

            return Ok(advertFavorites);
        }

        [HttpDelete("{advertId:Guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete(Guid advertId, CancellationToken cancellation)
        {
            await _advertFavoriteService.DeleteAsync(advertId, cancellation);

            return NoContent();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Add([FromBody]Guid advertId, CancellationToken cancellation)
        {
            await _advertFavoriteService.AddIfNotExistsAsync(advertId, cancellation);

            return Ok();
        }

        [HttpGet("ids")]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyCollection<Guid>>> GetIdsForCurrentUserId(CancellationToken cancellation)
        {
            var advertFavoriteIds = await _advertFavoriteService.GetIdsForCurrentUserAsync(cancellation);

            return Ok(advertFavoriteIds);
        }
    }
}
