using Board.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Contexts.Adverts
{
    public class AdvertFilterRequest
    {
        /// <summary>
        /// Идентификатор категории.
        /// </summary>
        public Guid? CategoryId { get; set; }

        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Текст поиска.
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// Минимальная цена.
        /// </summary>
        public decimal? minPrice { get; set; }

        /// <summary>
        /// Максимальная цена.
        /// </summary>
        public decimal? maxPrice { get; set; }

        /// <summary>
        /// Показатель высокого рейтинга владельца обьявления (4.0 и больше).
        /// </summary>
        public int? highRating { get; set; }

        /// <summary>
        /// Порядок сортировки.
        /// 1 - по убыванию
        /// </summary>
        public int? OrderDesc { get; set; }

        /// <summary>
        /// Вид сортировки
        /// date - сортировка по дате добавления.
        /// price - сортировка по цене.
        /// </summary>
        public string? SortBy { get; set; }
    }
}
