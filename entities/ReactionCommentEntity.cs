using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Blog.entities.enums;

namespace Blog.entities
{
    [Table("reaction_comment")]
    public class ReactionCommentEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong Id { get; set; }
        [Required] public string ApplicationUserId { get; set; } = string.Empty;

        [Required] public ulong CommentId { get; set; }        
        public LikeOrDislike Reaction  { get; set; }
        [JsonIgnore] public virtual CommentEntity? Comment { get; set; } 
        [JsonIgnore] public virtual ApplicationUser? ApplicationUser { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}