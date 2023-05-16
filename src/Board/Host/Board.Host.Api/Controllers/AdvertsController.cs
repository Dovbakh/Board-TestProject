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
    /// Работа с обьявлениями.
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
        /// Работа с обьявлениями.
        /// </summary>
        /// <param name="advertisementService">Сервис для работы с обьявлениями.</param>
        /// <param name="logger">Логгер.</param>
        public AdvertsController(IAdvertService advertisementService, ILogger<AdvertsController> logger)
        {
            _advertService = advertisementService;
            _logger = logger;
        }

        /// <summary>
        /// Получить все обьявления отсортированные по дате добавления по убыванию и с пагинацией.
        /// </summary>
        /// <param name="page">Номер страницы.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Коллекция элементов <see cref="AdvertSummary"/>.</returns>
        /// <response code="200">Запрос выполнен успешно.</response>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyCollection<AdvertSummary>>> GetAll(int? offset, int? limit, CancellationToken cancellation)
        {
            var result = await _advertService.GetAllAsync(offset, limit, cancellation);

            return Ok(result);
        }

        /// <summary>
        /// Получить все обьявления по фильтру и с пагинацией.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="page"></param>
        /// <param name="cancellation"></param>
        /// <returns>Элемент <see cref="AdvertSummary"/>.</returns>
        [HttpGet("by-filter")]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyCollection<AdvertSummary>>> GetAllFiltered([FromQuery] AdvertFilterRequest filter, int? offset, int? limit, 
            CancellationToken cancellation)
        {
            var result = await _advertService.GetAllFilteredAsync(filter, offset, limit, cancellation);

            return Ok(result);
        }

        /// <summary>
        /// Получить все обьявления по фильтру и с пагинацией.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="page"></param>
        /// <param name="cancellation"></param>
        /// <returns>Элемент <see cref="AdvertSummary"/>.</returns>
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
        /// <param name="id">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены</param>
        /// <returns>Элемент <see cref="AdvertDetails"/>.</returns>
        [HttpGet("{id:Guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<AdvertDetails>> GetById(Guid id, CancellationToken cancellation)
        {
            var result = await _advertService.GetByIdAsync(id, cancellation);

            return Ok(result);
        }

        /// <summary>
        /// Получить комментарии для обьявления с идентификатором.
        /// </summary>
        /// <param name="id">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены</param>
        /// <returns>Элемент <see cref="AdvertDetails"/>.</returns>
        [HttpGet("{id:Guid}/comments")]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyCollection<CommentDetails>>> GetCommentsByAdvertId(Guid id, int? offset, int? limit, CancellationToken cancellation)
        {
            var result = await _advertService.GetCommentsByAdvertIdAsync(id, offset, limit, cancellation);

            return Ok(result);
        }

        /// <summary>
        /// Добавить новое обьявление.
        /// </summary>
        /// <param name="addRequest">Элемент <see cref="AdvertAddRequest"/>.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Идентификатор нового обьявления.</returns>
        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] AdvertAddRequest addRequest, CancellationToken cancellation)
        {
            var advertId = await _advertService.CreateAsync(addRequest, cancellation);

            return CreatedAtAction(nameof(Create), advertId);
        }

        /// <summary>
        /// Изменить обьявление.
        /// </summary>
        /// <param name="id">Идентификатор обьявления.</param>
        /// <param name="updateRequest">Элемент <see cref="AdvertUpdateRequest"/>.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpPut("{id:Guid}")]
        public async Task<ActionResult<AdvertDetails>> Update(Guid id, [FromBody] AdvertUpdateRequest updateRequest, CancellationToken cancellation)
        {
            var result = await _advertService.UpdateAsync(id, updateRequest, cancellation);

            return Ok(result);
        }

        /// <summary>
        /// Удалить обьявление.
        /// </summary>
        /// <param name="id">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpDelete("soft/{id:Guid}")]
        public async Task<IActionResult> SoftDeleteById(Guid id, CancellationToken cancellation)
        {
            await _advertService.SoftDeleteAsync(id, cancellation);

            return NoContent();
        }

        /// <summary>
        /// Удалить обьявление.
        /// </summary>
        /// <param name="id">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpDelete("{id:Guid}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteById(Guid id, CancellationToken cancellation)
        {
            await _advertService.DeleteAsync(id, cancellation);

            return NoContent();
        }


    }

}
