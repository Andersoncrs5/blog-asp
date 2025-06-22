using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.DTOs.PlaylistItem
{
    public class UpdatePlaylistItemDTO
    {
        [Required] public ulong PlaylistItem { get; set; }
        [Required] public int Order { get; set; } 
    }
}