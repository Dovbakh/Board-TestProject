using Board.Application.AppData.Contexts.AdvertViews.Services;
using Board.Contracts.Conventions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Board.Host.Api.Controllers
{
    /// <summary>
    /// Контроллер для работы с просмотрами обьявлений.
    /// </summary>
    [ApiController]
    [Route("v2/[controller]")]
    [Produces("application/json")]
    [ApiConventionType(typeof(AppConventions))]
    public class AdvertViewsController : ControllerBase
    {
        private readonly IAdvertViewService _advertViewService;

        public AdvertViewsController(IAdvertViewService advertViewService)
        {
            _advertViewService = advertViewService;
        }

        /// <summary>
        /// Получить количество просмотров обьявления. [anonymous]
        /// </summary>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Количество просмотров.</returns>
        [HttpGet("{advertId:Guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<int>> GetCount(Guid advertId, CancellationToken cancellation)
        {
            var result = await _advertViewService.GetCountAsync(advertId, cancellation);

            return Ok(result);
        }

        /// <summary>
        /// Добавить просмотр обьявления, если еще не просмотрено. [anonymous]
        /// </summary>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Идентификатор просмотра.</returns>
        [HttpPost("{advertId:Guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<Guid>> AddIfNotExists(Guid advertId, CancellationToken cancellation)
        {
            var viewId = await _advertViewService.AddIfNotExistsAsync(advertId, cancellation);

            return CreatedAtAction(nameof(AddIfNotExists), viewId);
        }

    }
}
