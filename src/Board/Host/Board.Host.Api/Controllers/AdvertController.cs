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

namespace Board.Host.Api.Controllers
{
    /// <summary>
    /// Работа с обьявлениями.
    /// </summary>
    [ApiController]
    [Route("v1/advertisements")]
    [Produces("application/json")]
    [ApiConventionType(typeof(AppConventions))]
    
    public class AdvertController : ControllerBase
    {
        private readonly IAdvertService _advertService;
        private readonly ILogger<AdvertController> _logger;

        /// <summary>
        /// Работа с обьявлениями.
        /// </summary>
        /// <param name="advertisementService">Сервис для работы с обьявлениями.</param>
        /// <param name="logger">Логгер.</param>
        public AdvertController(IAdvertService advertisementService, ILogger<AdvertController> logger)
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
        [HttpGet("filter")]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyCollection<AdvertSummary>>> GetAllFiltered([FromQuery] AdvertFilterRequest filter, int? offset, int? limit, CancellationToken cancellation)
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
        /// Добавить новое обьявление.
        /// </summary>
        /// <param name="addRequest">Элемент <see cref="AdvertAddRequest"/>.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Идентификатор нового обьявления.</returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Guid>> Create([FromBody] AdvertAddRequest addRequest, CancellationToken cancellation)
        {
            var advertId = await _advertService.CreateAsync(addRequest, cancellation);
            return Ok(advertId);
            //return CreatedAtAction(nameof(GetById), new { id = advertId });
        }

        /// <summary>
        /// Изменить обьявление.
        /// </summary>
        /// <param name="id">Идентификатор обьявления.</param>
        /// <param name="updateRequest">Элемент <see cref="AdvertUpdateRequest"/>.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpPut("{id:Guid}")]
        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> SoftDeleteById(Guid id, CancellationToken cancellation)
        {
            await _advertService.DeleteAsync(id, cancellation);

            return NoContent();
        }

        /// <summary>
        /// Удалить обьявление.
        /// </summary>
        /// <param name="id">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpDelete("{id:Guid}")]
        [Authorize(Policy = "admin")]
        public async Task<IActionResult> DeleteById(Guid id, CancellationToken cancellation)
        {
            await _advertService.DeleteAsync(id, cancellation);

            return NoContent();
        }


    }

}
