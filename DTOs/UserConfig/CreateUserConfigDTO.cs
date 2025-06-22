using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blog.entities.enums;

namespace blog.DTOs.UserConfig
{
    public class CreateUserConfigDTO
    {
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
    }
}