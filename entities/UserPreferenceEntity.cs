using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Blog.entities;

namespace blog.entities
{
    [Table("user_preferences")] 
    public class UserPreferenceEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; } 

        [Required] public string ApplicationUserId { get; set; } = string.Empty;

        [Required] public long CategoryId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [ForeignKey(nameof(ApplicationUserId))]
        public virtual ApplicationUser? ApplicationUser { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public virtual CategoryEntity? Category { get; set; }
    }
}