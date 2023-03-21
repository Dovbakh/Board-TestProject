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
        public int Id { get; set; }
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
        public int AuthorId { get; set; }
        /// <summary>
        /// Идентификатор пользователя, которому оставлен отзыв.
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Идентификатор пользователя, который оставил отзыв.
        /// </summary>
        public int AdvertisementId { get; set; }
        /// <summary>
        /// Пользователь-автор.
        /// </summary>
        public User Author { get; set; }
        /// <summary>
        /// Пользователь, к которому оставлен отзыв.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Обьявление, к которому оставлен отзыв.
        /// </summary>
        public Advert Post { get; set; }
    }
}
