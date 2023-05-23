using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Options
{
    /// <summary>
    /// Опции HTTP-клиента для работы с микросервисом FileStorage
    /// </summary>
    public class FileClientOptions
    {
        /// <summary>
        /// Базовый адрес микросервиса FileStorage
        /// </summary>
        public string BasePath { get; set; }
    }
}
