using Board.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Contexts.Comments
{
    /// <summary>
    /// Модель фильтрации отзывов.
    /// </summary>
    public class CommentFilterRequest
    {
        /// <summary>
        /// Текст отзыва.
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// Идентификатор пользователя, которому оставили отзыв.
        /// </summary>
        public Guid? UserReceiverId { get; set; }

        /// <summary>
        /// Идентификатор пользователя, который оставил отзыв.
        /// </summary>
        public Guid? UserAuthorId { get; set; }

        /// <summary>
        /// Идентификатор обьявления, для которого оставлен отзыв.
        /// </summary>
        public Guid? AdvertId { get; set; }

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
