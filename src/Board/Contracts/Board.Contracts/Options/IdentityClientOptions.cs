using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Options
{
    /// <summary>
    /// Опции для авторизации IdentityServer
    /// </summary>
    public class IdentityClientOptions
    {
        /// <summary>
        /// Эндпоинт получения токена.
        /// </summary>
        public string GetTokenAddress { get; set; }

        /// <summary>
        /// Данные для получения токена внешним клиентом.
        /// </summary>
        public ExternalClientCredentials ExternalClientCredentials { get; set; }

        /// <summary>
        /// Данные для получения токена Board-клиентом.
        /// </summary>
        public InternalClientCredentials InternalClientCredentials { get; set; }

        /// <summary>
        /// Данные для проверка токена Board-клиентом.
        /// </summary>
        public ApiResourseCredentials ApiResourseCredentials { get; set; }
    }

    public class ExternalClientCredentials
    {
        public string Id { get; set; }
        public string Secret { get; set; }
        public string Scope { get; set; }
    }

    public class InternalClientCredentials
    {
        public string Id { get; set; }
        public string Secret { get; set; }
        public string Scope { get; set; }
    }

    public class ApiResourseCredentials
    {
        public string Id { get; set; }
        public string Secret { get; set; }
        public string Scope { get; set; }
    }
}