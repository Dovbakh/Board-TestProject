using Board.Application.AppData.Contexts.Comments.Services;
using Board.Contracts;
using Board.Contracts.Contexts.Comments;
using Board.Contracts.Conventions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Board.Host.Api.Controllers
{
    /// <summary>
    /// Работа с комментариями.
    /// </summary>
    [ApiController]
    [Route("v1/comments")]
    [Produces("application/json")]
    [ApiConventionType(typeof(AppConventions))]
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
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IReadOnlyCollection<CommentDetails>>> GetAll(int? offset, int? count, CancellationToken cancellation)
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
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyCollection<CommentDetails>>> GetAllFiltered([FromQuery] CommentFilterRequest filter, int take, int skip, CancellationToken cancellation)
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
        [HttpGet("{id:Guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<CommentDetails>> GetById(Guid id, CancellationToken cancellation)
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
        [Authorize]
        public async Task<ActionResult<Guid>> Create([FromBody] CommentAddRequest addRequest, CancellationToken cancellation)
        {
            var commentId = await _commentService.CreateAsync(addRequest, cancellation);

            return CreatedAtAction(nameof(GetById), new { id = commentId });
        }

        /// <summary>
        /// Изменить комментарий.
        /// </summary>
        /// <param name="id">Идентификатор комментария.</param>
        /// <param name="updateRequest">Элемент <see cref="CommentUpdateRequest"/>.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpPut("{id:Guid}")]
        [Authorize]
        public async Task<ActionResult<CommentDetails>> Update(Guid id, [FromBody] CommentUpdateRequest updateRequest, CancellationToken cancellation)
        {
            var result = await _commentService.UpdateAsync(id, updateRequest, cancellation);

            return Ok(result);
        }


        /// <summary>
        /// Удалить комментарий.
        /// </summary>
        /// <param name="id">Идентификатор комментария.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpDelete("{id:Guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteById(Guid id, CancellationToken cancellation)
        {
            await _commentService.SoftDeleteAsync(id, cancellation);

            return NoContent();
        }


    }
}
