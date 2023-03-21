using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Domain
{
    public class Advert
    {
        /// <summary>
        /// Идентификатор обьявления.
        /// </summary>
        public int Id { get; set; }

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
        /// Имя пользователя-автора обьявления.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Дата создания обьявления.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Дата истечения срока обьявления.
        /// </summary>
        public DateTime ExpireAt { get; set; }

        /// <summary>
        /// Количество просмотров обьявления.
        /// </summary>
        public int NumberOfViews { get; set; }

        /// <summary>
        /// Идентификатор категории обьявления.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Идентификатор пользователя-автора обьявления.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Категория обьявления.
        /// </summary>
        public Category Category { get; set; }

        /// <summary>
        /// Пользователь-автор обьявления.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Коллекция изображений обьявления.
        /// </summary>
        public ICollection<AdvertImage> PostImages { get; set; }

    }
}
