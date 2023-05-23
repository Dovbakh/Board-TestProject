using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Contexts.Images
{
    /// <summary>
    /// Модель данных изображения.
    /// </summary>
    public class ImageData
    {
        /// <summary>
        /// Имя изображения.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Контент изображения.
        /// </summary>
        public byte[] Content { get; set; }

        /// <summary>
        /// Тип контента изображения.
        /// </summary>
        public string ContentType { get; set; }
    }
}
