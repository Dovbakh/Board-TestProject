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
    [AllowAnonymous] // TODO: удалить
    [ApiConventionType(typeof(AppConventions))]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        public MessageController(IMessageService messageService)
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
        //[AllowAnonymous]
        public async Task<IActionResult> Send(MessageDetails message, CancellationToken cancellation)
        {
            await _messageService.SendAsync(message, cancellation);

            return Ok();
        }
    }
}
