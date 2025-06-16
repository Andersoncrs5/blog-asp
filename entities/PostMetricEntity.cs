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
        [Required] public long Likes { get; set; }
        [Required] public long DisLikes { get; set; }
        [Required] public long Shares { get; set; }
        [Required] public long CommentCount { get; set; }
        [Required] public long FavoriteCount { get; set; }
        [Required] public long Bookmarks { get; set; }
        [Required] public long Viewed { get; set; }
        [Required] public long ReportsReceivedCount { get; set; }
        [Required] public long EditedCount { get; set; }
        [Timestamp] public byte[] RowVersion { get; set; } = Array.Empty<byte>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

    }
}