using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Contracts.Clients.Images
{
    public class ImageShortInfoClientResponse
    {
        /// <summary>
        /// Идентификатор файла.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Наименование файла.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Наименование файла.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Дата создания файла.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Размер файла.
        /// </summary>
        public long Length { get; set; }
    }
}
