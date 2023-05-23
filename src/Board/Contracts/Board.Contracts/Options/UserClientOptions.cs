using Board.Domain;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Options
{
    /// <summary>
    /// Опции HTTP-клиента для работы с пользователями в микросервисе Identity
    /// </summary>
    public class UserClientOptions
    {
        /// <summary>
        /// Базовый адрес микросервиса Identity.
        /// </summary>
        public string BasePath { get; set; }
    }
}
