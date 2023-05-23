using FileStorage.Contracts.Conventions;
using FileStorage.Application.AppData.Contexts.Images.Services;
using FileStorage.Contracts.Contexts.Images;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileStorage.Host.Server.Controllers
{
    /// <summary>
    /// Работа с изображениями.
    /// </summary>
    [ApiController]
    [Route("v1/files")]
    [Produces("application/json")]
    [ApiConventionType(typeof(AppConventions))]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly ILogger<ImageController> _logger;

        /// <summary>
        /// Работа с изображениями.
        /// </summary>
        /// <param name="logger">Логгер.</param>
        public ImageController(IImageService imageService, ILogger<ImageController> logger)
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
        public async Task<ActionResult<ImageShortInfo>> GetInfo(Guid imageId, CancellationToken cancellation)
        {
            var result = await _imageService.GetInfoAsync(imageId, cancellation);

            return Ok(result);
        }

        /// <summary>
        /// Проверка на наличие изображение на сервере.
        /// </summary>
        /// <param name="imageId">Идентификатор изображения.</param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        [HttpGet("exists/{imageId:Guid}")]
        public async Task<ActionResult<bool>> IsImageExists(Guid imageId, CancellationToken cancellation)
        {
            var result = await _imageService.IsImageExists(imageId, cancellation);

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

            return Ok(fileName);
        }

        /// <summary>
        /// Скачать изображение по идентификатору. [anonymous]
        /// </summary>
        /// <param name="imageId">Идентификатор изображения.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Изображение.</returns>
        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<ImageData>> Download(Guid id, CancellationToken cancellation)
        {
            var result = await _imageService.DownloadAsync(id, cancellation);

            return Ok(result);
        }

        /// <summary>
        /// Удалить изображение. [admin only]
        /// </summary>
        /// <param name="imageId">Идентификатор изображения.</param>
        /// <param name="cancellation">Токен отмены.</param>
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellation)
        {
            await _imageService.DeleteAsync(id, cancellation);

            return NoContent();
        }

    }
}
