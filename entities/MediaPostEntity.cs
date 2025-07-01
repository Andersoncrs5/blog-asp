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
    [Table("media_post")]
    public class MediaPostEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong Id { get; set; }

        public long PostId { get; set; }
        public string Url { get; set; } = string.Empty;

        public MediaTypeEnum MediaType { get; set; }
        public string? Description { get; set; }
        public int? Order { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        [JsonIgnore] 
        [ForeignKey(nameof(PostId))]
        public virtual PostEntity? Post { get; set; }
    }
}