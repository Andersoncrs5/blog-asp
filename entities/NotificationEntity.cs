using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using blog.entities.enums;
using Blog.entities;

namespace blog.entities
{
    [Table("notifications")]
    public class NotificationEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id;

        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public NotificationTypeEnum NotificationType { get; set; }
        public bool IsRead { get; set; } = false;
        public string? RelatedEntityId { get; set; }
        public string? LinkUrl { get; set; } 
        public string? IconCssClass { get; set; }
        public string? SenderUserId { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [JsonIgnore] [ForeignKey(nameof(ApplicationUserId))] public virtual ApplicationUser? ApplicationUser { get; set; }
    }
}