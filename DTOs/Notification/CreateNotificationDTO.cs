using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using blog.entities.enums;

namespace blog.DTOs.Notification
{
    public class CreateNotificationDTO
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        [Required]
        [StringLength(1000)]
        public string Content { get; set; } = string.Empty;
        [Required]
        public NotificationTypeEnum NotificationType { get; set; }
        public long? RelatedEntityId { get; set; }
        [StringLength(50)]
        public string? RelatedEntityType { get; set; }
        [StringLength(500)]
        public string? LinkUrl { get; set; }
        [StringLength(100)]
        public string? IconCssClass { get; set; }
    }
}