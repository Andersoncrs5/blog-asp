using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blog.entities
{
    [Table("play_lists")]
    public class PlaylistEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong Id { get; set; }

        [Required] public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        [Required] public bool IsPublic { get; set; } = false;

        [Required] public string ApplicationUserId { get; set; } = string.Empty;

        [Timestamp] public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [JsonIgnore] public virtual ApplicationUser? ApplicationUser { get; set; }
        [JsonIgnore] public virtual ICollection<PlaylistItemEntity>? PlaylistItems { get; set; } = new List<PlaylistItemEntity>();
    }
}