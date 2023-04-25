using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Domain
{
    public class Comment
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
        /// Обьявление, к которому оставлен отзыв.
        /// </summary>
        public Advert Advert { get; set; }

        public bool isActive { get; set; }
    }
}
