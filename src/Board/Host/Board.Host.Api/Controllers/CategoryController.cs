using Board.Application.AppData.Contexts.Categories.Services;
using Board.Contracts;
using Board.Contracts.Contexts.Categories;
using Board.Contracts.Conventions;
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
    [Route("v1/categories")]
    [Produces("application/json")]
    [ApiConventionType(typeof(AppConventions))]
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
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyCollection<CategorySummary>>> GetAll(CancellationToken cancellation)
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
        [AllowAnonymous]
        public async Task<ActionResult<CategoryDetails>> GetById(Guid id, CancellationToken cancellation)
        {
            var result = await _categoryService.GetByIdAsync(id, cancellation);

            return Ok(result);
        }


        /// <summary>
        /// Создать новую категорию.
        /// </summary>
        /// <param name="createRequest">Модель запроса создания категории <see cref="CategoryAddRequest"/>.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <response code="201">Категория успешно создана.</response>
        /// <response code="400">Модель данных запроса невалидна.</response>
        /// <response code="422">Произошёл конфликт бизнес-логики.</response>
        /// <returns>Идентификатор новой категории.</returns>
        [HttpPost]
        //[Authorize(Roles = "admin")]
        public async Task<ActionResult<Guid>> Create([FromBody] CategoryAddRequest createRequest, CancellationToken cancellation)
        {
            var categoryId = await _categoryService.CreateAsync(createRequest, cancellation);

            return CreatedAtAction(nameof(GetById), new { id = categoryId });
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
        //[Authorize(Roles = "admin")]
        public async Task<ActionResult<CategoryDetails>> Update(Guid id, [FromBody] CategoryUpdateRequest updateRequest, CancellationToken cancellation)
        {
            var result = await _categoryService.UpdateAsync(id, updateRequest, cancellation);

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
        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteById(Guid id, CancellationToken cancellation)
        {
            await _categoryService.DeleteAsync(id, cancellation);

            return NoContent();
        }


    }
}

