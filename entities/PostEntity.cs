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

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public bool IsActived { get; set; }
        [Required] public string ApplicationUserId { get; set; } = string.Empty;

        [Required] public long CategoryId { get; set; }
        [Required] public long ReadTimes { get; set; } = 0;

        public double EngagementScore { get; set; } = 0.0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [JsonIgnore] public virtual PostMetricEntity? PostMetricEntity { get; set; }
        [JsonIgnore] public virtual ICollection<FavoritePostEntity>? FavoritePosts { get; set; }
        [JsonIgnore] [ForeignKey(nameof(ApplicationUserId))] public virtual ApplicationUser? ApplicationUser { get; set; }
        [JsonIgnore] [ForeignKey(nameof(CategoryId))] public virtual CategoryEntity? Category { get; set; }
        [JsonIgnore] public virtual ICollection<CommentEntity>? CommentEntities { get; set; }
        [JsonIgnore] public virtual ICollection<ReactionPostEntity>? ReactionPosts { get; set; }
        [JsonIgnore] public virtual ICollection<PlaylistItemEntity>? PlaylistItems { get; set; } = new List<PlaylistItemEntity>();
        [JsonIgnore] public virtual ICollection<MediaPostEntity>? MediaPostEntities { get; set; } = new List<MediaPostEntity>();
    }
}