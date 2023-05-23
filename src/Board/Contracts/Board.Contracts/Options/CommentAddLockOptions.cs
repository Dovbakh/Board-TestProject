using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Options
{
    /// <summary>
    /// Опции для блокировки ресурса при добавлении отзыва.
    /// </summary>
    public class CommentAddLockOptions
    {
        /// <summary>
        /// Префикс ресурса.
        /// </summary>
        public string CommentAddKey { get; set; } = "CommentAddKey_";

        /// <summary>
        /// Время истечения блокировки ресурса.
        /// </summary>
        public TimeSpan Expire { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Время ожидания разблокирования ресурса.
        /// </summary>
        public TimeSpan Wait { get; set; } = TimeSpan.FromSeconds(10);

        /// <summary>
        /// Время между попытками получения доступа к ресурсу.
        /// </summary>
        public TimeSpan Retry { get; set; } = TimeSpan.FromSeconds(1);
    }
}
