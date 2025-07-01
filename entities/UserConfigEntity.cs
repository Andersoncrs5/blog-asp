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
    [Table("user_configs")]
    public class UserConfigEntity
    {       
        [Key]
        [Required]
        public string ApplicationUserId { get; set; } = string.Empty;
        public string? ThemeName { get; set; } // Ex: "dark", "light", "custom"
        public string? PrimaryColor { get; set; } // Ex: "#RRGGBB"
        public string? SecondaryColor { get; set; }
        public string? AccentColor { get; set; }
        public FontTypeEnum? FontType { get; set; } 
        public int? FontSize { get; set; } // (ex: 14, 16)
        public decimal? LineHeight { get; set; } //  (ex: 1.5)
        public decimal? LetterSpacing { get; set; } // (ex: 0.05)
        public string? BorderColor { get; set; } 
        public int? BorderSize { get; set; } // (ex: 1, 2)
        public int? BorderRadius { get; set; } 
        public LayoutPreferenceEnum? LayoutPreference { get; set; }
        public bool? ShowProfilePictureInComments { get; set; } = true;
        public bool? EnableAnimations { get; set; } = true;
        public bool? NotificationsEnabled { get; set; } = true; 
        public string? TimeZone { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(ApplicationUserId))]
        public virtual ApplicationUser? ApplicationUser { get; set; }
    }
}