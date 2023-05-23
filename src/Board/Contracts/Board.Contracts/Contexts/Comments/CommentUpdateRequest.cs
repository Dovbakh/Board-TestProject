using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Contexts.Comments
{
    /// <summary>
    /// Модель изменения отзыва.
    /// </summary>
    public class CommentUpdateRequest
    {
        /// <summary>
        /// Текст отзыва.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Оценка, указанная пользователем.
        /// </summary>
        public int Rating { get; set; }

    }
}
