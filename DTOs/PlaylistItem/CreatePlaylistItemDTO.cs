using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.DTOs.PlaylistItem
{
    public class CreatePlaylistItemDTO
    {
        [Required] public long PostId { get; set; }
        [Required] public ulong Playlist { get; set; }
        public int? Order { get; set; } 
    }
}