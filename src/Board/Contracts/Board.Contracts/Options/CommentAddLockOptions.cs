using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Options
{
    public class CommentAddLockOptions
    {
        public string CommentAddKey { get; set; } = "CommentAddKey_";
        public TimeSpan Expire { get; set; } = TimeSpan.FromSeconds(30);
        public TimeSpan Wait { get; set; } = TimeSpan.FromSeconds(10);
        public TimeSpan Retry { get; set; } = TimeSpan.FromSeconds(1);
    }
}
