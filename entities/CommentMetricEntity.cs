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
        [Required] public ulong reportCount { get; set; } = 0;
        [Required] public ulong editedTimes { get; set; } = 0;
        [Required] public ulong favoritesCount { get; set; } = 0;
        [Required] public ulong repliesCount { get; set; } = 0;
        [Required] public ulong viewsCount { get; set; } = 0;
        [Timestamp] public byte[] RowVersion { get; set; } = Array.Empty<byte>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}