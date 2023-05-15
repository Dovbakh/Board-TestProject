using Board.Application.AppData.Contexts.AdvertFavorites.Services;
using Board.Contracts.Contexts.AdvertFavorites;
using Board.Contracts.Contexts.Adverts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Board.Host.Api.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    [Produces("application/json")]
    [AllowAnonymous]
    public class AdvertFavoriteController : ControllerBase
    {
        private readonly IAdvertFavoriteService _advertFavoriteService;

        public AdvertFavoriteController(IAdvertFavoriteService advertFavoriteService)
        {
            _advertFavoriteService = advertFavoriteService;
        }

        [HttpGet("{userId:Guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyCollection<Guid>>> GetAllByUserId(CancellationToken cancellation)
        {
            var favoriteList = await _advertFavoriteService.GetAllForCurrentUserAsync(cancellation);

            return Ok(favoriteList);
        }

        [HttpDelete("{favoriteId:Guid}")]
        public async Task<IActionResult> Delete(Guid favoriteId, CancellationToken cancellation)
        {
            await _advertFavoriteService.DeleteAsync(favoriteId, cancellation);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Guid advertId, CancellationToken cancellation)
        {
            await _advertFavoriteService.AddIfNotExistsAsync(advertId, cancellation);

            return Ok();
        }
    }
}
