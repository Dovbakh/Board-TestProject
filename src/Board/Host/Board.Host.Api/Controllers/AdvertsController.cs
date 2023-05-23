using Board.Application.AppData.Contexts.Adverts.Services;
using Board.Contracts;
using Board.Contracts.Contexts.Categories;
using Board.Contracts.Contexts.Adverts;
using Board.Contracts.Conventions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Board.Contracts.Contexts.Comments;

namespace Board.Host.Api.Controllers
{
    /// <summary>
    /// Контроллер для работы с обьявлениями.
    /// </summary>
    [ApiController]
    [Route("v2/[controller]")]
    [Produces("application/json")]
    [ApiConventionType(typeof(AppConventions))]
    public class AdvertsController : ControllerBase
    {
        private readonly IAdvertService _advertService;
        private readonly ILogger<AdvertsController> _logger;

        /// <summary>
        /// Конструктор контроллера работы с обьявлениями.
        /// </summary>
        /// <param name="advertisementService">Сервис для работы с обьявлениями.</param>
        /// <param name="logger">Логгер.</param>
        public AdvertsController(IAdvertService advertisementService, ILogger<AdvertsController> logger)
        {
            _advertService = advertisementService;
            _logger = logger;
        }

        /// <summary>
        /// Получить все обьявления отсортированные по дате добавления по убыванию и с пагинацией. [anonymous]
        /// </summary>
        /// <param name="offset">Количество пропускаемых обьявлений.</param>
        /// <param name="limit">Количество получаемых обьявлений.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Список обьявлений.</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyCollection<AdvertSummary>>> GetAll(int? offset, int? limit, CancellationToken cancellation)
        {
            var result = await _advertService.GetAllAsync(offset, limit, cancellation);

            return Ok(result);
        }

        /// <summary>
        /// Получить все обьявления по фильтру и с пагинацией. [anonymous]
        /// </summary>
        /// <param name="filter">Модель фильтрации обьявлений.</param>
        /// <param name="offset">Количество пропускаемых обьявлений.</param>
        /// <param name="limit">Количество получаемых обьявлений.</param>
        /// <param name="cancellation"></param>
        /// <returns>Список обьявлений.</returns>
        [HttpGet("by-filter")]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyCollection<AdvertSummary>>> GetAllFiltered([FromQuery] AdvertFilterRequest filter, int? offset, int? limit, 
            CancellationToken cancellation)
        {
            var result = await _advertService.GetAllFilteredAsync(filter, offset, limit, cancellation);

            return Ok(result);
        }

        /// <summary>
        /// Получить все обьявления по фильтру и с пагинацией. [anonymous]
        /// </summary>
        /// <param name="filter">Модель фильтрации обьявлений.</param>
        /// <param name="offset">Количество пропускаемых обьявлений.</param>
        /// <param name="limit">Количество получаемых обьявлений.</param>
        /// <param name="cancellation"></param>
        /// <returns>Список обьявлений.</returns>
        [HttpPost("by-filter")]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyCollection<AdvertSummary>>> GetAllFilteredBody([FromBody] AdvertFilterRequest filter, int? offset, int? limit,
            CancellationToken cancellation)
        {
            var result = await _advertService.GetAllFilteredAsync(filter, offset, limit, cancellation);

            return Ok(result);
        }


        /// <summary>
        /// Получить обьявление по идентификатору.
        /// </summary>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены</param>
        /// <returns>Элемент <see cref="AdvertDetails"/>.</returns>
        [HttpGet("{advertId:Guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<AdvertDetails>> GetById(Guid advertId, CancellationToken cancellation)
        {
            var result = await _advertService.GetByIdAsync(advertId, cancellation);

            return Ok(result);
        }

        /// <summary>
        /// Получить отзывы для обьявления по идентификатору с пагинацией. [anonymous]
        /// </summary>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="offset">Количество пропускаемых отзывов.</param>
        /// <param name="limit">Количество получаемых отзывов.</param>
        /// <param name="cancellation">Токен отмены</param>
        /// <returns>Список отзывов.</returns>
        [HttpGet("{advertId:Guid}/comments")]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyCollection<CommentDetails>>> GetCommentsByAdvertId(Guid advertId, int? offset, int? limit, CancellationToken cancellation)
        {
            var result = await _advertService.GetCommentsByAdvertIdAsync(advertId, offset, limit, cancellation);

            return Ok(result);
        }

        /// <summary>
        /// Получить отзывы для обьявления по идентификатору с пагинацией и фильтрацией. [anonymous]
        /// </summary>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="filterRequest">Модель фильтрации обьявлений.</param>
        /// <param name="offset">Количество пропускаемых отзывов.</param>
        /// <param name="limit">Количество получаемых отзывов.</param>
        /// <param name="cancellation">Токен отмены</param>
        /// <returns>Список отзывов.</returns>
        [HttpPost("{advertId:Guid}/comments/by-filter")]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyCollection<CommentDetails>>> GetFilteredCommentsByAdvertId(Guid advertId, [FromBody]CommentFilterRequest filterRequest, int? offset, int? limit, CancellationToken cancellation)
        {
            var result = await _advertService.GetFilteredCommentsByAdvertIdAsync(advertId, filterRequest, offset, limit, cancellation);

            return Ok(result);
        }

        /// <summary>
        /// Добавить новое обьявление. [authorize]
        /// </summary>
        /// <param name="addRequest">Модель добавления обьявления.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Идентификатор нового обьявления.</returns>
        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] AdvertAddRequest addRequest, CancellationToken cancellation)
        {
            var advertId = await _advertService.CreateAsync(addRequest, cancellation);

            return CreatedAtAction(nameof(Create), advertId);
        }

        /// <summary>
        /// Изменить обьявление. [authorize]
        /// </summary>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="updateRequest">Модель изменения обьявления.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpPut("{advertId:Guid}")]
        public async Task<ActionResult<AdvertDetails>> Update(Guid advertId, [FromBody] AdvertUpdateRequest updateRequest, CancellationToken cancellation)
        {
            var result = await _advertService.UpdateAsync(advertId, updateRequest, cancellation);

            return Ok(result);
        }

        /// <summary>
        /// Удалить обьявление, сделав его неактивным. [authorize]
        /// </summary>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpDelete("soft/{advertId:Guid}")]
        public async Task<IActionResult> SoftDeleteById(Guid advertId, CancellationToken cancellation)
        {
            await _advertService.SoftDeleteAsync(advertId, cancellation);

            return NoContent();
        }

        /// <summary>
        /// Удалить обьявление. [admin]
        /// </summary>
        /// <param name="advertId">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpDelete("{advertId:Guid}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteById(Guid advertId, CancellationToken cancellation)
        {
            await _advertService.DeleteAsync(advertId, cancellation);

            return NoContent();
        }
    }

}
