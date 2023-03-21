using Board.Application.AppData.Contexts.Posts.Services;
using Board.Contracts.Contexts.Posts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Board.Host.Api.Controllers
{
    /// <summary>
    /// Работа с обьявлениями.
    /// </summary>
    [ApiController]
    [Route("v1/[controller]")]
    public class AdvertController : ControllerBase
    {
        private readonly IAdvertService _postService;
        private readonly ILogger<AdvertController> _logger;

        /// <summary>
        /// Работа с обьявлениями.
        /// </summary>
        /// <param name="advertisementService">Сервис для работы с обьявлениями.</param>
        /// <param name="logger">Логгер.</param>
        public AdvertController(IAdvertService advertisementService, ILogger<AdvertController> logger)
        {
            _postService = advertisementService;
            _logger = logger;
        }

        /// <summary>
        /// Получить все обьявления отсортированные по дате добавления по убыванию и с пагинацией.
        /// </summary>
        /// <param name="page">Номер страницы.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Коллекция элементов <see cref="AdvertSummary"/>.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<AdvertSummary>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(int? offset, int? limit, CancellationToken cancellation)
        {
            var result = await _postService.GetAllAsync(offset, limit, cancellation);

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
        [ProducesResponseType(typeof(IReadOnlyCollection<AdvertSummary>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllFiltered([FromQuery] AdvertFilterRequest filter, int? offset, int? count, CancellationToken cancellation)
        {
            var result = await _postService.GetAllFilteredAsync(filter, offset, count, cancellation);

            return Ok(result);
        }


        /// <summary>
        /// Получить обьявление по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены</param>
        /// <returns>Элемент <see cref="AdvertDetails"/>.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(AdvertDetails), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellation)
        {
            var result = await _postService.GetByIdAsync(id, cancellation);

            return Ok(result);
        }

        ///// <summary>
        ///// Получить обьявление по идентификатору.
        ///// </summary>
        ///// <param name="id">Идентификатор обьявления.</param>
        ///// <param name="cancellation">Токен отмены</param>
        ///// <returns>Элемент <see cref="AdvertisementResponseDto"/>.</returns>
        //[HttpGet("history")]
        //[ProducesResponseType(typeof(AdvertisementResponseDto), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[AllowAnonymous]
        //public async Task<IActionResult> GetHistory(int? page, CancellationToken cancellation)
        //{
        //    var result = await _postService.GetHistoryAsync(page, cancellation);

        //    return Ok(result);
        //}


        ///// <summary>
        ///// Получить обьявление по идентификатору.
        ///// </summary>
        ///// <param name="id">Идентификатор обьявления.</param>
        ///// <param name="cancellation">Токен отмены</param>
        ///// <returns>Элемент <see cref="AdvertisementResponseDto"/>.</returns>
        //[HttpGet("last-viewed")]
        //[ProducesResponseType(typeof(AdvertisementResponseDto), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[AllowAnonymous]
        //public async Task<IActionResult> GetLastViewed(int? count, CancellationToken cancellation)
        //{
        //    var result = await _postService.GetLastViewedAsync(count, cancellation);

        //    return Ok(result);
        //}


        /// <summary>
        /// Добавить новое обьявление.
        /// </summary>
        /// <param name="addRequest">Элемент <see cref="AdvertAddRequest"/>.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Идентификатор нового обьявления.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        //[Authorize]
        public async Task<IActionResult> Add([FromForm] AdvertAddRequest addRequest, CancellationToken cancellation)
        {
            var advertisementId = await _postService.AddAsync(addRequest, cancellation);
            return CreatedAtAction(nameof(GetById), new { id = advertisementId }, addRequest);
            //return Created("", advertisementId);
        }

        /// <summary>
        /// Изменить обьявление.
        /// </summary>
        /// <param name="id">Идентификатор обьявления.</param>
        /// <param name="updateRequest">Элемент <see cref="AdvertUpdateRequest"/>.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [Authorize]
        public async Task<IActionResult> Update([FromForm] AdvertUpdateRequest updateRequest, int id, CancellationToken cancellation)
        {
            await _postService.UpdateAsync(id, updateRequest, cancellation);

            return NoContent();
        }

        /// <summary>
        /// Удалить обьявление.
        /// </summary>
        /// <param name="id">Идентификатор обьявления.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellation)
        {
            await _postService.DeleteAsync(id, cancellation);

            return NoContent();
        }


    }

}
