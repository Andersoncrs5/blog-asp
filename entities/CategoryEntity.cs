using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using blog.entities;

namespace Blog.entities
{
    [Table("categories")]
    public class CategoryEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public bool IsActived { get; set; } = true;

        [Required] public string ApplicationUserId { get; set; } = string.Empty;

        [Timestamp]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [JsonIgnore] public virtual ApplicationUser? ApplicationUser { get; set; }
        [JsonIgnore] public virtual ICollection<PostEntity>? Posts { get; set; }
        [JsonIgnore] public virtual ICollection<UserPreferenceEntity>? UserPreferenceEntities { get; set; } = new List<UserPreferenceEntity>();
    }
}