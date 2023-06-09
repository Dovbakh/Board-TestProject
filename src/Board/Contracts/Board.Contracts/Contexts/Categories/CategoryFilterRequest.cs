﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Contexts.Categories
{
    /// <summary>
    /// Модель фильтрации категории.
    /// </summary>
    public class CategoryFilterRequest
    {
        /// <summary>
        /// Название категории.
        /// </summary>  
        public string Name { get; set; }

        /// <summary>
        /// Идентификатор родительской категории.
        /// </summary>
        public Guid? ParentId { get; set; }
    }
}
