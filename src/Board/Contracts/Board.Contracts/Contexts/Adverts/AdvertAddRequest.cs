using Board.Domain;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Contexts.Adverts
{
    public class AdvertAddRequest
    {
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
        /// Номер телефона, указанный в обьявлении.
        /// </summary>
        public string Phone { get; set; }


        /// <summary>
        /// Идентификатор категории обьявления.
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Идентификатор пользователя-автора обьявления.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Коллекция изображений обьявления.
        /// </summary>
        //public ICollection<IFormFile> AdvertImages { get; set; }
    }
}
