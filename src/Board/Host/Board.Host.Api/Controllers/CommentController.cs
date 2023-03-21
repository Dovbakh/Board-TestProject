using Board.Application.AppData.Contexts.Comments.Services;
using Board.Contracts.Contexts.Comments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Board.Host.Api.Controllers
{
    /// <summary>
    /// Работа с комментариями.
    /// </summary>
    [ApiController]
    [Route("v1/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        /// <summary>
        /// Работа с комментариями.
        /// </summary>
        /// <param name="commentService">Сервис для работы с комментариями.</param>
        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        /// <summary>
        /// Получить все комментарии по фильтру и с пагинацией.
        /// </summary>
        /// <param name="page">Номер страницы с комментариями.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Коллекция элементов <see cref="CommentDto"/>.</returns>
        [HttpGet("comments")]
        [ProducesResponseType(typeof(IReadOnlyCollection<CommentDetails>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(int? offset, int? count, CancellationToken cancellation)
        {
            var result = await _commentService.GetAllAsync(offset, count, cancellation);

            return Ok(result);
        }

        /// <summary>
        /// Получить все комментарии по фильтру и с пагинацией.
        /// </summary>
        /// <param name="page">Номер страницы с комментариями.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Коллекция элементов <see cref="CommentDto"/>.</returns>
        [HttpGet("filter")]
        [ProducesResponseType(typeof(IReadOnlyCollection<CommentDetails>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllFiltered([FromQuery] CommentFilterRequest filter, int take, int skip, CancellationToken cancellation)
        {
            var result = await _commentService.GetAllFilteredAsync(filter, take, skip, cancellation);

            return Ok(result);
        }


        /// <summary>
        /// Получить комментарий по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор комментария.</param>
        /// <param name="cancellation">Токен отмены</param>
        /// <returns>Элемент <see cref="CommentDetails"/>.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CommentDetails), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellation)
        {
            var result = await _commentService.GetByIdAsync(id, cancellation);

            return Ok(result);
        }


        /// <summary>
        /// Добавить новый комментарий.
        /// </summary>
        /// <param name="addRequest">Элемент <see cref="CommentAddRequest"/>.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Идентификатор нового комментария.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]

        [Authorize]
        public async Task<IActionResult> Add(CommentAddRequest addRequest, CancellationToken cancellation)
        {
            var commentId = await _commentService.AddAsync(addRequest, cancellation);

            return CreatedAtAction(nameof(GetById), new { id = commentId }, addRequest);
        }

        /// <summary>
        /// Изменить комментарий.
        /// </summary>
        /// <param name="id">Идентификатор комментария.</param>
        /// <param name="updateRequest">Элемент <see cref="CommentUpdateRequest"/>.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [Authorize]
        public async Task<IActionResult> Update(int id, CommentUpdateRequest updateRequest, CancellationToken cancellation)
        {
            await _commentService.UpdateAsync(id, updateRequest, cancellation);

            return NoContent();
        }

        /// <summary>
        /// Удалить комментарий.
        /// </summary>
        /// <param name="id">Идентификатор комментария.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellation)
        {
            await _commentService.DeleteAsync(id, cancellation);

            return NoContent();
        }


    }
}
