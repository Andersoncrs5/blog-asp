using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blog.entities
{
    [Table("playlist_items")]
    public class PlaylistItemEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong Id { get; set; }
        [Required] public ulong PlaylistId { get; set; }
        [Required] public long PostId { get; set; }

        public int? Order { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore] public virtual PlaylistEntity? Playlist { get; set; }
        [JsonIgnore] public virtual PostEntity? Post { get; set; }
    }
}