using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blog.entities
{
    [Table("comments")]
    public class CommentEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong Id { get; set; }

        [Required] public string ApplicationUserId { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        [Required] public long PostId { get; set; }

        public ulong? ParentId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [JsonIgnore] public virtual ApplicationUser? ApplicationUser { get; set; }
        [JsonIgnore] public virtual PostEntity? Post { get; set; }
        [JsonIgnore] public virtual CommentEntity? ParentComment { get; set; }
        public virtual CommentMetricEntity? CommentMetric { get; set; }
        [JsonIgnore] public virtual ICollection<CommentEntity>? Replies { get; set; } = new List<CommentEntity>();
        [JsonIgnore] public virtual ICollection<FavoriteCommentEntity>? FavoriteCommentEntities { get; set; }
        [JsonIgnore] public virtual ICollection<ReactionCommentEntity>? ReactionComments { get; set; }
    }
}