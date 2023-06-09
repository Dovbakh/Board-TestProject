﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Domain
{
    /// <summary>
    /// Обьявление.
    /// </summary>
    public class Advert
    {
        /// <summary>
        /// Идентификатор обьявления.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название обьявления.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Текст обьявления.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Цена, указанная в обьявлении.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Адрес, указанный в обьявлении.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Дата создания обьявления.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Идентификатор категории обьявления.
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Идентификатор пользователя-автора обьявления.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Категория обьявления.
        /// </summary>
        public Category Category { get; set; }

        /// <summary>
        /// Коллекция изображений обьявления.
        /// </summary>
        public ICollection<AdvertImage> AdvertImages { get; set; }

        /// <summary>
        /// Коллекция избранных обьявлений.
        /// </summary>
        public ICollection<AdvertFavorite> AdvertFavorites { get; set; }

        /// <summary>
        /// Коллекция просмотров обьявления.
        /// </summary>
        public ICollection<AdvertView> AdvertViews { get; set; }

        /// <summary>
        /// Статус обьявления.
        /// </summary>
        public bool IsActive { get; set; }

    }
}
