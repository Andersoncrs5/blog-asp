using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.DTOs.Playlist
{
    public class CreatePlaylistDTO
    {
        [Required]
        [StringLength(maximumLength:150, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [StringLength(maximumLength:1000)]
        public string? Description { get; set; } = string.Empty;

        [StringLength(maximumLength:1000)]
        [Url]
        public string? ImageUrl { get; set; } = string.Empty;

        
    }
}