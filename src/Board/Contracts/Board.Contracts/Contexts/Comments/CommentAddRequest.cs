using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Contexts.Comments
{
    public class CommentAddRequest
    {
        /// <summary>
        /// Текст отзыва.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Оценка, указанная пользователем.
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// Идентификатор пользователя, который оставил отзыв.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Идентификатор обьявления, для которого оставлен отзыв.
        /// </summary>
        public Guid AdvertId { get; set; }
    }
}
