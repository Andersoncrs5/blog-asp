using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.DTOs.PlaylistItem
{
    public class CreatePlaylistItemDTO
    {
        [Required] public ulong PostId { get; set; }
        public int? Order { get; set; } 
    }
}