﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Contracts.Contexts.Users
{
    public class UserUpdateRequest
    {
        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Номер телефона пользователя.
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Адрес пользователя.
        /// </summary>
        public string? Address { get; set; }
    }
}
