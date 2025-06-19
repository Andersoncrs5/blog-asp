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
        [Required] public long Likes { get; set; } = 0;
        [Required] public long DisLikes { get; set; } = 0;
        [Required] public long Shares { get; set; } = 0;
        [Required] public long CommentCount { get; set; } = 0;
        [Required] public long FavoriteCount { get; set; } = 0;
        [Required] public long Bookmarks { get; set; } = 0;
        [Required] public long Viewed { get; set; } = 0;
        [Required] public long ReportsReceivedCount { get; set; } = 0;
        [Required] public long EditedCount { get; set; } = 0;
        [Required] public long MediaCount { get; set; } = 0;
        [Timestamp] public byte[] RowVersion { get; set; } = Array.Empty<byte>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

    }
}