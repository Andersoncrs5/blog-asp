using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.entities
{
    [Table("user_metric")]
    public class UserMetricEntity
    {
        [Key] 
        public string ApplicationUserId { get; set; } = string.Empty;

        [Column("likes_given_count_in_comment")]
        [Required] 
        public long LikesGivenCountInComment { get; set; } = 0;

        [Column("dislikes_given_count_in_comment")]
        [Required] 
        public long DeslikesGivenCountInComment { get; set; } = 0;

        [Required] 
        [Column("likes_given_count_in_post")]
        public long LikesGivenCountInPost { get; set; } = 0;

        [Required] 
        [Column("deslikes_given_count_in_post")]
        public long DeslikesGivenCountInPost { get; set; } = 0;

        [Required] 
        [Column("followers_count")]
        public long FollowersCount { get; set; } = 0;

        [Required] 
        [Column("following_count")]
        public long FollowingCount { get; set; } = 0;

        [Required] 
        [Column("posts_count")]
        public long PostsCount { get; set; } = 0;

        [Required] 
        [Column("comments_count")]
        public long CommentsCount { get; set; } = 0;

        [Required] 
        [Column("shares_count")]
        public long SharesCount { get; set; } = 0;

        [Required] 
        [Column("reports_received_count")] 
        public long ReportsReceivedCount { get; set; } = 0;

        [Required] 
        [Column("media_uploads_count")] 
        public long MediaUploadsCount { get; set; } = 0;

        [Required] 
        [Column("saved_posts_count")] 
        public long SavedPostsCount { get; set; } = 0;

        [Required] 
        [Column("saved_comments_count")] 
        public long SavedCommentsCount { get; set; } = 0;
        
        [Required] 
        [Column("saved_media_count")] 
        public long SavedMediaCount { get; set; } = 0;

        [Required] 
        [Column("edited_count")] 
        public long EditedCount { get; set; } = 0;

        [Required] 
        [Column("profile_views")] 
        public long ProfileViews { get; set; } = 0;

        [Timestamp]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        public DateTime? LastLogin { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}