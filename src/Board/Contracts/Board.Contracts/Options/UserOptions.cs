﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Options
{
    public class UserOptions
    {
        public int ListDefaultCount { get; set; } = 10;
        public int AdvertListDefaultCount { get; set; } = 10;
        public int CommentListDefaultCount { get; set; } = 10;
    }
}
