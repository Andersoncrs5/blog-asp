using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using blog.entities;
using Microsoft.AspNetCore.Identity;

namespace Blog.entities
{
    public class ApplicationUser: IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        [JsonIgnore] public virtual UserMetricEntity? UserMetric { get; set; }
        [JsonIgnore] public virtual ICollection<CategoryEntity>? Categories { get; set; }
        [JsonIgnore] public virtual ICollection<PostEntity>? Posts { get; set; }
        [JsonIgnore] public virtual ICollection<FavoritePostEntity>? FavoritePosts { get; set; }
        [JsonIgnore] public virtual ICollection<CommentEntity>? CommentEntities { get; set; }
        [JsonIgnore] public virtual ICollection<FavoriteCommentEntity>? FavoriteCommentEntities { get; set; }
        [JsonIgnore] public virtual ICollection<ReactionPostEntity>? ReactionPosts { get; set; }
        [JsonIgnore] public virtual ICollection<ReactionCommentEntity>? ReactionComments { get; set; }
        [JsonIgnore] public virtual ICollection<PlaylistEntity>? PlaylistEntities { get; set; }
        [JsonIgnore] public virtual RecoverAccountEntity? RecoverAccountEntities { get; set; }
        [JsonIgnore] public virtual ICollection<FollowEntity>? Following { get; set; } = new List<FollowEntity>();
        [JsonIgnore] public virtual ICollection<FollowEntity>? Followers { get; set; } = new List<FollowEntity>();
    }
}