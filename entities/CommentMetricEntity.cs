using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blog.entities
{
    public class CommentMetricEntity
    {
        [Key] public ulong CommentId { get; set; }
        public ulong Likes { get; set; } = 0;
        public ulong DisLikes { get; set; } = 0;
        public ulong ReportCount { get; set; } = 0;
        public ulong EditedTimes { get; set; } = 0;
        public ulong FavoritesCount { get; set; } = 0;
        public ulong RepliesCount { get; set; } = 0;
        public ulong ViewsCount { get; set; } = 0;
        [Timestamp] public byte[] RowVersion { get; set; } = Array.Empty<byte>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        [JsonIgnore] public virtual CommentEntity? Comment { get; set; }
    }
}