using Board.Application.AppData.Contexts.Categories.Services;
using Board.Contracts.Contexts.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Board.Host.Api.Controllers
{
    /// <summary>
    /// Работа с категориями.
    /// </summary>
    [ApiController]
    [Route("v1/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        /// <summary>
        /// Работа с категориями.
        /// </summary>
        /// <param name="categoryService">Сервис для работы с категориями</param>
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Получить все категории с пагинацией.
        /// </summary>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Коллекция элементов <see cref="CategoryDetails"/>.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<CategoryDetails>), StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(CancellationToken cancellation)
        {
            var result = await _categoryService.GetAllAsync(cancellation);

            return Ok(result);
        }

        /// <summary>
        /// Получить категорию по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор категории.</param>
        /// <param name="cancellation">Токен отмены</param>
        /// <returns>Элемент <see cref="CategoryDetails"/>.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CategoryDetails), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellation)
        {
            var result = await _categoryService.GetByIdAsync(id, cancellation);

            return Ok(result);
        }


        /// <summary>
        /// Добавить новую категорию.
        /// </summary>
        /// <param name="categoryDto">Элемент <see cref="CategoryDto"/>.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Идентификатор новой категории.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Add(CategoryDetails categoryDto, CancellationToken cancellation)
        {
            var categoryId = await _categoryService.AddAsync(categoryDto, cancellation);

            return CreatedAtAction(nameof(GetById), new { id = categoryId }, categoryDto);
        }

        /// <summary>
        /// Изменить категорию.
        /// </summary>
        /// <param name="id">Идентификатор категории.</param>
        /// <param name="categoryDto">Элемент <see cref="CategoryDetails"/>.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, CategoryDetails categoryDto, CancellationToken cancellation)
        {
            await _categoryService.UpdateAsync(id, categoryDto, cancellation);

            return NoContent();
        }

        /// <summary>
        /// Удалить категорию.
        /// </summary>
        /// <param name="id">Идентификатор категории.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellation)
        {
            await _categoryService.DeleteAsync(id, cancellation);

            return NoContent();
        }


    }
}

