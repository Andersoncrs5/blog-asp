using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blog.entities
{
    [Table("posts")]
    public class PostEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [StringLength(300)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(3000)]
        public string Content { get; set; } = string.Empty;

        [Required] public bool IsActived { get; set; } = true;

        [Required] public string ApplicationUserId { get; set; } = string.Empty;

        [Required] public long categoryId { get; set; }
        [Required] public long ReadTimes { get; set; } = 0;

        [Timestamp]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [JsonIgnore] public virtual PostMetricEntity? PostMetricEntity { get; set; }

        [JsonIgnore] public virtual ICollection<FavoritePostEntity>? FavoritePosts { get; set; }

        [JsonIgnore] public virtual ICollection<CommentEntity>? CommentEntities { get; set; }
        [JsonIgnore] public virtual ICollection<ReactionPostEntity>? ReactionPosts { get; set; }
        [JsonIgnore] public virtual ICollection<PlaylistItemEntity>? PlaylistItems { get; set; } = new List<PlaylistItemEntity>();
    }
}