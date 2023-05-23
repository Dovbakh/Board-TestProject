using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notifier.Application.AppData.Contexts.Messages.Services;
using Notifier.Contracts.Contexts.Messages;
using Notifier.Contracts.Conventions;

namespace Notifier.Host.Server.Controllers
{
    /// <summary>
    /// Работа с пользователями.
    /// </summary>
    [ApiController]
    [Route("v1/[controller]")]
    [Produces("application/json")]
    [ApiConventionType(typeof(AppConventions))]
    public class MessageController : ControllerBase
    {
        private readonly INotificationService _messageService;
        public MessageController(INotificationService messageService)
        {
            _messageService = messageService;
        }


        /// <summary>
        /// Получить все обьявления отсортированные по дате добавления по убыванию и с пагинацией.
        /// </summary>
        /// <param name="page">Номер страницы.</param>
        /// <param name="cancellation">Токен отмены.</param>
        /// <returns>Коллекция элементов <see cref="AdvertSummary"/>.</returns>
        /// <response code="200">Запрос выполнен успешно.</response>
        [HttpPost]
        public async Task<IActionResult> Send(NotificationDetails message, CancellationToken cancellation)
        {
            await _messageService.SendAsync(message, cancellation);

            return Ok();
        }
    }
}
