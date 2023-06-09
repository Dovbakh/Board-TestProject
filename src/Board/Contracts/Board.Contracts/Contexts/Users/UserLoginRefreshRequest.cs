﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Board.Contracts.Contexts.Users
{
    /// <summary>
    /// Модель логина с рефреш токеном пользователя.
    /// </summary>
    public class UserLoginRefreshRequest
    {
        /// <summary>
        /// Рефреш токен.
        /// </summary>
        [BindProperty(Name = "refresh_token", SupportsGet = true)]
        public string RefreshToken { get; set; }
    }
}
