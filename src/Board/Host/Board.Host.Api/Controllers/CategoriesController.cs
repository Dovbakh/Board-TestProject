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
    [ApiController]
    [Route("v2/[controller]")]
    [Produces("application/json")]
    [ApiConventionType(typeof(AppConventions))]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        /// <summary>
        /// Конструктор для контроллеры работы с категориями.
        /// </summary>
        /// <param name="categoryService">Сервис категорий.</param>
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Получить список всех категорий. [anonymous]
        /// </summary>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Список категорий.</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyCollection<CategorySummary>>> GetAll(CancellationToken cancellation)
        {
            var result = await _categoryService.GetAllAsync(cancellation);
           
            return Ok(result);
        }

        /// <summary>
        /// Получить категорию по идентификатору. [anonymous]
        /// </summary>
        /// <param name="categoryId">Идентификатор.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Категория.</returns>
        [HttpGet("{categoryId:Guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<CategoryDetails>> GetById(Guid categoryId, CancellationToken cancellation)
        {
            var result = await _categoryService.GetByIdAsync(categoryId, cancellation);

            return Ok(result);
        }


        /// <summary>
        /// Создать новую категорию. [admin]
        /// </summary>
        /// <param name="createRequest">Модель запроса создания категории <see cref="CategoryAddRequest"/>.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Идентификатор новой категории.</returns>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<Guid>> Create([FromBody] CategoryAddRequest createRequest, CancellationToken cancellation)
        {
            var categoryId = await _categoryService.CreateAsync(createRequest, cancellation);

            return CreatedAtAction(nameof(Create), categoryId);
        }

        /// <summary>
        /// Обновить категорию. [admin]
        /// </summary>
        /// <param name="categoryId">Идентификатор.</param>
        /// <param name="updateRequest">Модель запроса обновления категории.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Обновленная категория.</returns>
        [HttpPut("{categoryId:Guid}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<CategoryDetails>> Update(Guid categoryId, [FromBody] CategoryUpdateRequest updateRequest, CancellationToken cancellation)
        {
            var result = await _categoryService.UpdateAsync(categoryId, updateRequest, cancellation);

            return Ok(result);
        }

        /// <summary>
        /// Удалить категорию по идентификатору. [admin]
        /// </summary>
        /// <param name="categoryId">Идентификатор.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpDelete("{categoryId:Guid}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteById(Guid categoryId, CancellationToken cancellation)
        {
            await _categoryService.DeleteAsync(categoryId, cancellation);

            return NoContent();
        }


    }
}

