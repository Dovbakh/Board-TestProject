using Board.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Contexts.AdvertImages
{
    public class AdvertImageDto
    {
        /// <summary>
        /// Идентификатор изображения.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор обьявления.
        /// </summary>
        public Guid AdvertId { get; set; }

        /// <summary>
        /// Идентификатор обьявления.
        /// </summary>
        public Guid FileId { get; set; }

        /// <summary>
        /// Дата загрузки изображения.
        /// </summary>
        public DateTime CreatedAt { get; set; }

    }
}
