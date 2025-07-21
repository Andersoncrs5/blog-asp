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
    [Table("reaction_posts")]
    public class ReactionPostEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong Id { get; set; }
        [Required] public string ApplicationUserId { get; set; } = string.Empty;
        [Required] public long PostId { get; set; }
        public LikeOrDislike Reaction  { get; set; }
        [JsonIgnore] [ForeignKey(nameof(ApplicationUserId))] public virtual ApplicationUser? ApplicationUser { get; set; } 
        [ForeignKey(nameof(PostId))] public virtual PostEntity? Post { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}