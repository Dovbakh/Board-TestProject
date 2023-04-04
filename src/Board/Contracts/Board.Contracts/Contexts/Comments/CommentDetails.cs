using Board.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Contexts.Comments
{
    public class CommentDetails
    {
        /// <summary>
        /// Идентификатор отзыва.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Текст отзыва.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Оценка, указанная пользователем.
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// Дата отзыва.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Идентификатор пользователя, который оставил отзыв.
        /// </summary>
        public Guid AuthorId { get; set; }

        /// <summary>
        /// Идентификатор пользователя, которому оставлен отзыв.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Идентификатор обьявления, для которого оставлен отзыв.
        /// </summary>
        public Guid AdvertisementId { get; set; }

        /// <summary>
        /// Пользователь-автор.
        /// </summary>
        public User Author { get; set; }
    }
}
