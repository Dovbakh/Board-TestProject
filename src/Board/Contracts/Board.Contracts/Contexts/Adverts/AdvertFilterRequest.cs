using Board.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Board.Contracts.Contexts.Adverts
{
    /// <summary>
    /// Модель фильтрации обьявления.
    /// </summary>
    public class AdvertFilterRequest
    {
        /// <summary>
        /// Идентификатор категории.
        /// </summary>
        [BindProperty(Name = "category-id", SupportsGet = true)]
        public Guid? CategoryId { get; set; }

        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        [BindProperty(Name = "user-id", SupportsGet = true)]
        public Guid? UserId { get; set; }

        /// <summary>
        /// Текст поиска.
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// Минимальная цена.
        /// </summary>
        [BindProperty(Name = "min-price", SupportsGet = true)]
        public decimal? MinPrice { get; set; }

        /// <summary>
        /// Максимальная цена.
        /// </summary>
        [BindProperty(Name = "max-price", SupportsGet = true)]
        public decimal? MaxPrice { get; set; }

        /// <summary>
        /// Показатель высокого рейтинга владельца обьявления (4.0 и больше).
        /// </summary>
        [BindProperty(Name = "high-rating", SupportsGet = true)]
        public bool? HighRating { get; set; }

        /// <summary>
        /// Порядок сортировки.
        /// 1 - по убыванию
        /// </summary>
        [BindProperty(Name = "order-desc", SupportsGet = true)]
        public int? OrderDesc { get; set; }

        /// <summary>
        /// Вид сортировки
        /// date - сортировка по дате добавления.
        /// price - сортировка по цене.
        /// </summary>
        [BindProperty(Name = "sort-by", SupportsGet = true)]
        public string? SortBy { get; set; }
    }
}
