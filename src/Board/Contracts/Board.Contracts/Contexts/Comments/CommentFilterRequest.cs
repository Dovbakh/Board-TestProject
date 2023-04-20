using Board.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Contexts.Comments
{
    public class CommentFilterRequest
    {
        /// <summary>
        /// Текст отзыва.
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// Дата отзыва.
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Идентификатор пользователя, который оставил отзыв.
        /// </summary>
        public Guid? AuthorId { get; set; }

        /// <summary>
        /// Идентификатор пользователя, которому оставлен отзыв.
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Идентификатор обьявления, для которого оставлен отзыв.
        /// </summary>
        public Guid? AdvertisementId { get; set; }

        /// <summary>
        /// Порядок сортировки.
        /// 1 - по убыванию
        /// </summary>
        public int? OrderDesc { get; set; }

        /// <summary>
        /// Вид сортировки
        /// date - сортировка по дате добавления.
        /// rating - сортировка по оценке комментария.
        /// </summary>
        public string? SortBy { get; set; }
    }
}
