using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.entities
{
    public class CommentMetricEntity
    {
        [Key] public ulong CommentId { get; set; }
        [Required] public ulong Likes { get; set; } = 0;
        [Required] public ulong DisLikes { get; set; } = 0;
        [Required] public ulong ReportCount { get; set; } = 0;
        [Required] public ulong EditedTimes { get; set; } = 0;
        [Required] public ulong FavoritesCount { get; set; } = 0;
        [Required] public ulong RepliesCount { get; set; } = 0;
        [Required] public ulong ViewsCount { get; set; } = 0;
        [Timestamp] public byte[] RowVersion { get; set; } = Array.Empty<byte>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}