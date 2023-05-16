using FileStorage.Contracts.Conventions;
using FileStorage.Application.AppData.Contexts.Images.Services;
using FileStorage.Contracts.Contexts.Images;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileStorage.Host.Server.Controllers
{
    /// <summary>
    /// Работа с обьявлениями.
    /// </summary>
    [ApiController]
    [Route("v1/files")]
    [Produces("application/json")]
    [ApiConventionType(typeof(AppConventions))]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _fileService;
        private readonly ILogger<ImageController> _logger;

        /// <summary>
        /// Работа с обьявлениями.
        /// </summary>
        /// <param name="advertisementService">Сервис для работы с обьявлениями.</param>
        /// <param name="logger">Логгер.</param>
        public ImageController(IImageService fileService, ILogger<ImageController> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        /// <summary>
        /// Получить все обьявления отсортированные по дате добавления по убыванию и с пагинацией.
        /// </summary>
        /// <param name="page">Номер страницы.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Коллекция элементов <see cref="AdvertSummary"/>.</returns>
        /// <response code="200">Запрос выполнен успешно.</response>
        [HttpGet("info/{id:Guid}")]
        public async Task<ActionResult<ImageShortInfo>> GetInfo(Guid id, CancellationToken cancellation)
        {
            var result = await _fileService.GetInfoAsync(id, cancellation);

            return Ok(result);
        }

        [HttpGet("exists/{id:Guid}")]
        public async Task<ActionResult<bool>> IsImageExists(Guid id, CancellationToken cancellation)
        {
            var result = await _fileService.IsImageExists(id, cancellation);

            return Ok(result);
        }

        /// <summary>
        /// Получить все обьявления отсортированные по дате добавления по убыванию и с пагинацией.
        /// </summary>
        /// <param name="page">Номер страницы.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Коллекция элементов <see cref="AdvertSummary"/>.</returns>
        /// <response code="200">Запрос выполнен успешно.</response>
        [HttpPost]
        public async Task<ActionResult<Guid>> Upload(IFormFile file, CancellationToken cancellation)
        {
            var fileName = await _fileService.UploadAsync(file, cancellation);

            return Ok(fileName);
        }

        /// <summary>
        /// Получить все обьявления отсортированные по дате добавления по убыванию и с пагинацией.
        /// </summary>
        /// <param name="page">Номер страницы.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Коллекция элементов <see cref="AdvertSummary"/>.</returns>
        /// <response code="200">Запрос выполнен успешно.</response>
        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<ImageData>> Download(Guid id, CancellationToken cancellation)
        {
            var result = await _fileService.DownloadAsync(id, cancellation);

            return Ok(result);
        }

        /// <summary>
        /// Получить все обьявления отсортированные по дате добавления по убыванию и с пагинацией.
        /// </summary>
        /// <param name="page">Номер страницы.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Коллекция элементов <see cref="AdvertSummary"/>.</returns>
        /// <response code="200">Запрос выполнен успешно.</response>
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellation)
        {
            await _fileService.DeleteAsync(id, cancellation);

            return NoContent();
        }

    }
}
