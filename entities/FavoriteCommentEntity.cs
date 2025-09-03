using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blog.entities
{
    [Table("favorite_comment")]
    public class FavoriteCommentEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong Id { get; set; }

        [Required] public string ApplicationUserId { get; set; } = string.Empty;

        [JsonIgnore] 
        [ForeignKey(nameof(ApplicationUserId))] 
        public virtual ApplicationUser? ApplicationUser { get; set; } 

        [Required] public ulong CommentId { get; set; }        
        
        [ForeignKey(nameof(CommentId))] public CommentEntity? Comment { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}