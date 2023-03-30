using Board.Application.AppData.Contexts.Categories.Services;
using Board.Contracts;
using Board.Contracts.Contexts.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Board.Host.Api.Controllers
{
    /// <summary>
    /// Контроллер для работы с категориями.
    /// </summary>
    /// <response code="500">Произошла внутренняя ошибка.</response>
    [ApiController]
    [Route("v1/[controller]")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        /// <summary>
        /// Работа с категориями.
        /// </summary>
        /// <param name="categoryService">Сервис категорий.</param>
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Получить список всех категорий.
        /// </summary>
        /// <param name="cancellation">Токен отмены.</param>
        /// <response code="200">Запрос выполнен успешно.</response>
        /// <returns>Список моделей категорий <see cref="CategorySummary"/>.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<CategorySummary>), StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(CancellationToken cancellation)
        {
            var result = await _categoryService.GetAllAsync(cancellation);
           
            return Ok(result);
        }

        /// <summary>
        /// Получить категорию по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <response code="200">Запрос выполнен успешно.</response>
        /// <response code="404">Категория с указанным идентификатором не найдена.</response>
        /// <returns>Модель категории <see cref="CategoryDetails"/>.</returns>
        [HttpGet("{id:Guid}")]
        [ProducesResponseType(typeof(CategoryDetails), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellation)
        {
            var result = await _categoryService.GetByIdAsync(id, cancellation);

            return Ok(result);
        }


        /// <summary>
        /// Создать новую категорию.
        /// </summary>
        /// <param name="createRequest">Модель запроса создания категории <see cref="CategoryCreateRequest"/>.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <response code="201">Категория успешно создана.</response>
        /// <response code="400">Модель данных запроса невалидна.</response>
        /// <response code="422">Произошёл конфликт бизнес-логики.</response>
        /// <returns>Идентификатор новой категории.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status422UnprocessableEntity)]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody]CategoryCreateRequest createRequest, CancellationToken cancellation)
        {
            var categoryId = await _categoryService.CreateAsync(createRequest, cancellation);
            // TODO: createRequest или cancellationToken
            return CreatedAtAction(nameof(GetById), new { id = categoryId }, createRequest);
        }

        /// <summary>
        /// Обновить категорию.
        /// </summary>
        /// <param name="id">Идентификатор.</param>
        /// <param name="updateRequest">Модель запроса обновления категории <see cref="CategoryUpdateRequest"/>.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <response code="200">Запрос выполнен успешно.</response>
        /// <response code="400">Модель данных запроса невалидна.</response>
        /// <response code="403">Доступ запрещён.</response>
        /// <response code="404">Категория с указанным идентификатором не найдена.</response>
        /// <response code="422">Произошёл конфликт бизнес-логики.</response>
        /// <returns>Модель обновленной категории <see cref="CategoryDetails"/>.</returns>
        [HttpPut("{id:Guid}")]
        [ProducesResponseType(typeof(CategoryDetails), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status422UnprocessableEntity)]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(Guid id, CategoryUpdateRequest updateRequest, CancellationToken cancellation)
        {
            await _categoryService.UpdateAsync(id, updateRequest, cancellation);
            var result = await _categoryService.GetByIdAsync(id, cancellation);

            return Ok(result);
        }

        /// <summary>
        /// Частично обновить категорию.
        /// </summary>
        /// <param name="id">Идентификатор.</param>
        /// <param name="updateRequest">Модель запроса обновления категории <see cref="CategoryUpdateRequest"/>.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <response code="200">Запрос выполнен успешно.</response>
        /// <response code="400">Модель данных запроса невалидна.</response>
        /// <response code="403">Доступ запрещён.</response>
        /// <response code="404">Категория с указанным идентификатором не найдена.</response>
        /// <response code="422">Произошёл конфликт бизнес-логики.</response>
        /// <returns>Модель обновленной категории <see cref="CategoryDetails"/>.</returns>
        [HttpPatch("{id:Guid}")]
        [ProducesResponseType(typeof(CategoryDetails), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status403Forbidden)]    
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status422UnprocessableEntity)]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Patch(Guid id, [FromBody] JsonPatchDocument<CategoryUpdateRequest> updateRequest, CancellationToken cancellation)
        {
            // TODO: патч в сервисе для категории
            // TODO: указать правильный тип входного параметра?
            //await _categoryService.UpdateAsync(id, categoryDto, cancellation);
            var result = await _categoryService.GetByIdAsync(id, cancellation);

            return Ok(result);
        }

        /// <summary>
        /// Удалить категорию по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <response code="204">Запрос выполнен успешно.</response>
        /// <response code="403">Доступ запрещён.</response>
        /// <response code="404">Категория с указанным идентификатором не найдена.</response>
        [HttpDelete("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellation)
        {
            await _categoryService.DeleteAsync(id, cancellation);

            return NoContent();
        }


    }
}

