using Board.Application.AppData.Contexts.Files.Services;
using Board.Contracts.Contexts.Adverts;
using Board.Contracts.Contexts.Files;
using Board.Contracts.Conventions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Board.Host.Api.Controllers
{
    /// <summary>
    /// Работа с пользователями.
    /// </summary>
    [ApiController]
    [Route("v1/[controller]")]
    [Produces("application/json")]
    [ApiConventionType(typeof(AppConventions))]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly ILogger<FileController> _logger;

        /// <summary>
        /// Работа с обьявлениями.
        /// </summary>
        /// <param name="advertisementService">Сервис для работы с обьявлениями.</param>
        /// <param name="logger">Логгер.</param>
        public FileController(IFileService fileService, ILogger<FileController> logger)
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
        //[AllowAnonymous]
        public async Task<ActionResult<FileShortInfo>> GetInfo(Guid id, CancellationToken cancellation)
        {
            var result = await _fileService.GetInfoAsync(id, cancellation);

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
        //[AllowAnonymous]
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
        //[AllowAnonymous]
        public async Task<ActionResult> Download(Guid id, CancellationToken cancellation)
        {
            var result = await _fileService.DownloadAsync(id, cancellation);

            return File(result.Content, result.ContentType, result.Name, true);
        }

        /// <summary>
        /// Получить все обьявления отсортированные по дате добавления по убыванию и с пагинацией.
        /// </summary>
        /// <param name="page">Номер страницы.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Коллекция элементов <see cref="AdvertSummary"/>.</returns>
        /// <response code="200">Запрос выполнен успешно.</response>
        [HttpDelete("{id:Guid}")]
        //[AllowAnonymous]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellation)
        {
            await _fileService.DeleteAsync(id, cancellation);

            return NoContent();
        }
    }
}
