﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Domain
{
    public class AdvertImage
    {
        /// <summary>
        /// Идентификатор изображения.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор обьявления.
        /// </summary>
        public int PostId { get; set; }

        /// <summary>
        /// Обьявление, к которому относится изображение.
        /// </summary>
        public Advert Post { get; set; }
    }
}