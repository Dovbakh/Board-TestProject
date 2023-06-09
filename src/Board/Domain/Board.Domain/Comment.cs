﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Domain
{
    /// <summary>
    /// Отзыв.
    /// </summary>
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
        public Guid UserAuthorId { get; set; }

        /// <summary>
        /// Идентификатор обьявления, для которого оставлен отзыв.
        /// </summary>
        public Guid AdvertId { get; set; }

        /// <summary>
        /// Обьявление, к которому оставлен отзыв.
        /// </summary>
        public Advert Advert { get; set; }

        /// <summary>
        /// Статус отзыва.
        /// </summary>
        public bool IsActive { get; set; }
    }
}
