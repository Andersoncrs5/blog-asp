using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Blog.entities;

namespace blog.entities
{
    [Table("follows")]
    public class FollowEntity
    {
        [Required]
        public string FollowerId { get; set; } = string.Empty;

        [Required]
        public string FollowedId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        [ForeignKey(nameof(FollowerId))] 
        public virtual ApplicationUser? Follower { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(FollowedId))] 
        public virtual ApplicationUser? Followed { get; set; }
    }
}