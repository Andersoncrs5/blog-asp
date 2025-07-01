using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.entities
{
    [Table("post_metric")]
    public class PostMetricEntity
    {
        [Key] public long PostId { get; set; }
        public ulong Likes { get; set; } = 0;
        public ulong DisLikes { get; set; } = 0;
        public ulong Shares { get; set; } = 0;
        public ulong CommentCount { get; set; } = 0;
        public ulong FavoriteCount { get; set; } = 0;
        public ulong Bookmarks { get; set; } = 0;
        public ulong Viewed { get; set; } = 0;
        public ulong ReportsReceivedCount { get; set; } = 0;
        public ulong EditedCount { get; set; } = 0;
        public ulong MediaCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

    }
}