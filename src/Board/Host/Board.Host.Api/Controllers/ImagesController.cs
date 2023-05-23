using Board.Application.AppData.Contexts.Images.Services;
using Board.Contracts.Contexts.Adverts;
using Board.Contracts.Contexts.Images;
using Board.Contracts.Conventions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Board.Host.Api.Controllers
{
    /// <summary>
    /// Контроллер для работы с изображениями.
    /// </summary>
    [ApiController]
    [Route("v2/[controller]")]
    [Produces("application/json")]
    [ApiConventionType(typeof(AppConventions))]
    public class ImagesController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly ILogger<ImagesController> _logger;

        /// <summary>
        /// Конструктор для контроллера работы с изображениями.
        /// </summary>
        /// <param name="imageService">Сервис для работы с изображениями.</param>
        /// <param name="logger">Логгер.</param>
        public ImagesController(IImageService imageService, ILogger<ImagesController> logger)
        {
            _imageService = imageService;
            _logger = logger;
        }

        /// <summary>
        /// Получить краткую информацию об изображении. [anonymous]
        /// </summary>
        /// <param name="imageId">Идентификатор изоюражения.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Краткая информация об изображении.</returns>
        [HttpGet("info/{imageId:Guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<ImageShortInfo>> GetInfo(Guid imageId, CancellationToken cancellation)
        {
            var result = await _imageService.GetInfoAsync(imageId, cancellation);

            return Ok(result);
        }

        /// <summary>
        /// Загрузить изображение на сервер. [authorize]
        /// </summary>
        /// <param name="file">Файл с изображением.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Идентификатор изображения.</returns>
        [HttpPost]
        public async Task<ActionResult<Guid>> Upload(IFormFile file, CancellationToken cancellation)
        {
            var fileName = await _imageService.UploadAsync(file, cancellation);

            return CreatedAtAction(nameof(Upload), fileName);
        }

        /// <summary>
        /// Скачать изображение по идентификатору. [anonymous]
        /// </summary>
        /// <param name="imageId">Идентификатор изображения.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Изображение.</returns>
        [HttpGet("{imageId:Guid}")]
        [AllowAnonymous]
        public async Task<FileResult> Download(Guid imageId, CancellationToken cancellation)
        {
            var result = await _imageService.DownloadAsync(imageId, cancellation);

            return File(result.Content, result.ContentType, result.Name, true);
        }

        /// <summary>
        /// Удалить изображение. [admin only]
        /// </summary>
        /// <param name="imageId">Идентификатор изображения.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpDelete("{imageId:Guid}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(Guid imageId, CancellationToken cancellation)
        {
            await _imageService.DeleteAsync(imageId, cancellation);

            return NoContent();
        }
    }
}
