using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blog.entities
{
    [Table("favorite_post")]
    public class FavoritePostEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required] public string ApplicationUserId { get; set; } = string.Empty;

        [JsonIgnore] public virtual ApplicationUser? ApplicationUser { get; set; } 

        [Required] public long PostId { get; set; }        
        
        [JsonIgnore] [ForeignKey(nameof(PostId))] public virtual PostEntity? Post { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}