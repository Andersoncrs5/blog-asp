using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using blog.entities;
using Blog.entities;
using Blog.entities.enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Blog.Context
{
    public class AppDbContext: IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) {}

        public DbSet<UserMetricEntity> UserMetrics { get; set; }
        public DbSet<CategoryEntity> CategoryEntities { get; set; }
        public DbSet<PostEntity> PostEntities { get; set; }
        public DbSet<PostMetricEntity> PostMetricEntities { get; set; }
        public DbSet<FavoritePostEntity> FavoritePostEntities { get; set; }
        public DbSet<CommentEntity> CommentEntities { get; set; }
        public DbSet<CommentMetricEntity> CommentMetricEntities { get; set; }
        public DbSet<FavoriteCommentEntity> FavoriteCommentEntities { get; set; }
        public DbSet<ReactionPostEntity> ReactionPostEntities { get; set; }
        public DbSet<ReactionCommentEntity> ReactionCommentEntities { get; set; }
        public DbSet<PlaylistEntity> PlaylistEntities { get; set; }
        public DbSet<PlaylistItemEntity> PlaylistItemEntities { get; set; }
        public DbSet<RecoverAccountEntity> RecoverAccountEntities { get; set; }
        public DbSet<MediaPostEntity> MediaPostEntities  { get; set; }
        public DbSet<FollowEntity> FollowsEntities { get; set; }
        public DbSet<UserPreferenceEntity> UserPreferenceEntities { get; set; }
        public DbSet<UserConfigEntity> UserConfigEntities { get; set; }
        public DbSet<NotificationEntity> NotificationEntities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLazyLoadingProxies();
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserConfigEntity>(entity =>
            {
                entity.HasKey(uc => uc.ApplicationUserId);

                entity.HasOne(uc => uc.ApplicationUser) 
                      .WithOne(u => u.UserConfig)       
                      .HasForeignKey<UserConfigEntity>(uc => uc.ApplicationUserId) 
                      .OnDelete(DeleteBehavior.Cascade); 

                entity.Property(uc => uc.ThemeName).HasMaxLength(50).IsRequired(false);
                entity.Property(uc => uc.PrimaryColor).HasMaxLength(7).IsRequired(false);
                entity.Property(uc => uc.SecondaryColor).HasMaxLength(7).IsRequired(false);
                entity.Property(uc => uc.AccentColor).HasMaxLength(7).IsRequired(false);
                entity.Property(uc => uc.BorderColor).HasMaxLength(7).IsRequired(false);
                entity.Property(uc => uc.BorderSize).HasMaxLength(50).IsRequired(false);
                entity.Property(uc => uc.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();
                entity.Property(uc => uc.UpdatedAt).IsRequired(false);
            });

            builder.Entity<UserPreferenceEntity>(entity =>
            {
                entity.HasKey(up => up.Id); 
                entity.Property(up => up.Id).HasColumnType("bigint").ValueGeneratedOnAdd();

                entity.Property(up => up.ApplicationUserId).IsRequired();
                entity.Property(up => up.CategoryId).IsRequired();
                entity.Property(up => up.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();
                
                entity.HasOne(up => up.ApplicationUser)
                      .WithMany(u => u.UserPreferenceEntities) 
                      .HasForeignKey(up => up.ApplicationUserId)
                      .OnDelete(DeleteBehavior.Cascade); 
                
                entity.HasOne(up => up.Category)
                      .WithMany(c => c.UserPreferenceEntities) 
                      .HasForeignKey(up => up.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict); 
            });

            builder.Entity<FollowEntity>(entity =>
            {
                entity.HasKey(f => new { f.FollowerId, f.FollowedId });

                entity.Property(f => f.CreatedAt)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP")
                      .ValueGeneratedOnAdd();

                entity.HasOne(f => f.Follower)
                      .WithMany(u => u.Following) 
                      .HasForeignKey(f => f.FollowerId)
                      .OnDelete(DeleteBehavior.Restrict); 
                                                            
                entity.HasOne(f => f.Followed)
                      .WithMany(u => u.Followers) 
                      .HasForeignKey(f => f.FollowedId)
                      .OnDelete(DeleteBehavior.Restrict);                
            });

            builder.Entity<PostEntity>()
                .HasMany(u => u.MediaPostEntities)
                .WithOne(e => e.Post)
                .HasForeignKey(f => f.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>(entity => 
            {
                entity.HasKey(u => u.Id);
            });   

            builder.Entity<MediaPostEntity>(entity => 
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(c => c.Url);
                entity.HasIndex(c => c.MediaType);
                entity.Property(e => e.Id).HasColumnType("bigint");
                entity.Property(e => e.PostId).IsRequired(true);
                entity.Property(e => e.Url).IsRequired(true).HasMaxLength(1250);
                entity.Property(e => e.Description).IsRequired(false).HasMaxLength(600);
                entity.Property(e => e.Order).IsRequired(false).HasColumnType("smallint");
                entity.Property(e => e.MediaType).IsRequired(true).HasDefaultValue(MediaTypeEnum.IMAGE);
                entity.Property(e => e.UpdatedAt).IsRequired(false);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();
            });

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.PlaylistEntities)
                .WithOne(e => e.ApplicationUser)
                .HasForeignKey(f => f.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
                .HasOne(u => u.RecoverAccountEntities)
                .WithOne(r => r.User)
                .HasForeignKey<RecoverAccountEntity>(r => r.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<RecoverAccountEntity>(entity =>
            {
                entity.HasKey(e => e.ApplicationUserId);
                entity.Property(e => e.Token).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.ExpireAt).IsRequired();
                entity.Property(e => e.BlockedAt).IsRequired(false);
                entity.Property(e => e.IsUsed).HasDefaultValue(false).IsRequired(true);
                entity.Property(e => e.FailedAttempts).HasDefaultValue(0).IsRequired(true);
                entity.Property(e => e.RequestIpAddress).HasMaxLength(100).IsRequired(false);
                entity.Property(e => e.RequestUserAgent).HasMaxLength(500).IsRequired(false);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();
            });

            builder.Entity<PlaylistEntity>(entity => 
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnType("bigint");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();
                entity.Property(e => e.UpdatedAt).IsRequired(false);
                entity.Property(e => e.ImageUrl).IsRequired(false).HasMaxLength(2000);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Description).IsRequired(false).HasMaxLength(1000);
                entity.Property(e => e.IsPublic).IsRequired().HasDefaultValue(false);
            });

            builder.Entity<PostEntity>()
                .HasMany(e => e.PlaylistItems)
                .WithOne(e => e.Post)
                .HasForeignKey(e => e.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PlaylistItemEntity>(entity => 
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PostId).HasColumnType("bigint");
                entity.Property(e => e.Id).HasColumnType("bigint");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();
                entity.Property(e => e.Order).IsRequired(false);

                entity.HasOne(pi => pi.Playlist)
                      .WithMany(p => p.PlaylistItems) 
                      .HasForeignKey(pi => pi.PlaylistId)
                      .OnDelete(DeleteBehavior.Restrict);
               
                entity.HasIndex(pi => new { pi.PlaylistId, pi.PostId }).IsUnique();
            });

            builder.Entity<ApplicationUser>()
                .HasOne(u => u.UserMetric)
                .WithOne()
                .HasForeignKey<UserMetricEntity>(u => u.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CommentEntity>()
                .HasOne(c => c.CommentMetric)
                .WithOne(cm => cm.Comment)
                .HasForeignKey<CommentMetricEntity>(c => c.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.FavoriteCommentEntities)
                .WithOne(fc => fc.ApplicationUser)
                .HasForeignKey(f => f.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.ReactionComments)
                .WithOne(e => e.ApplicationUser)
                .HasForeignKey(f => f.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CommentEntity>()
                .HasMany(c => c.ReactionComments)
                .WithOne(e => e.Comment)
                .HasForeignKey(f => f.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.ReactionPosts)
                .WithOne(rp => rp.ApplicationUser)
                .HasForeignKey(f => f.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CommentEntity>()
                .HasMany(c => c.FavoriteCommentEntities)
                .WithOne(fc => fc.Comment)
                .HasForeignKey(f => f.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserMetricEntity>(entity =>
            {
                entity.HasKey(u => u.ApplicationUserId);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();
                entity.Property(e => e.UpdatedAt).IsRequired(false);
                entity.HasIndex(um => um.ProfileViews);
                entity.HasIndex(um => um.ApplicationUserId);
                entity.HasIndex(um => um.PlaylistCount);
                entity.HasIndex(um => um.PreferenceCount);
                entity.Property(e => e.PlaylistCount).HasColumnType("bigint").HasDefaultValue(0).IsRequired();
            });
                
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Categories)
                .WithOne(c => c.ApplicationUser) 
                .HasForeignKey(c => c.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Posts)
                .WithOne(p => p.ApplicationUser)
                .HasForeignKey(c => c.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.FavoritePosts)
                .WithOne(fp => fp.ApplicationUser) 
                .HasForeignKey(f => f.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.CommentEntities)
                .WithOne(c => c.ApplicationUser)
                .HasForeignKey(c => c.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PostEntity>()
                .HasMany(p => p.ReactionPosts)
                .WithOne(r => r.Post)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CategoryEntity>()
                .HasMany(p => p.Posts)
                .WithOne(c => c.Category) 
                .HasForeignKey(u => u.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CategoryEntity>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();
                entity.Property(e => e.UpdatedAt).IsRequired(false);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
                entity.Property(e => e.IsActived).IsRequired().HasDefaultValue(true);
            });

            builder.Entity<PostEntity>()
                .HasOne(p => p.PostMetricEntity)
                .WithOne() 
                .HasForeignKey<PostMetricEntity>(u => u.PostId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<PostEntity>()
                .HasMany(p => p.FavoritePosts)
                .WithOne(f => f.Post) 
                .HasForeignKey(f => f.PostId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<PostEntity>()
                .HasMany(p => p.CommentEntities)
                .WithOne(c => c.Post)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PostMetricEntity>(entity =>
            {
                entity.HasKey(p => p.PostId);
                entity.Property(p => p.UpdatedAt).IsRequired(false);
                entity.Property(p => p.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();
                entity.Property(e => e.Likes).HasColumnType("bigint").IsRequired(true).HasDefaultValue(0);
                entity.Property(e => e.DisLikes).HasColumnType("bigint").IsRequired(true).HasDefaultValue(0);
                entity.Property(e => e.Shares).HasColumnType("bigint").IsRequired(true).HasDefaultValue(0);
                entity.Property(e => e.CommentCount).HasColumnType("bigint").IsRequired(true).HasDefaultValue(0);
                entity.Property(e => e.FavoriteCount).HasColumnType("bigint").IsRequired(true).HasDefaultValue(0);
                entity.Property(e => e.Bookmarks).HasColumnType("bigint").IsRequired(true).HasDefaultValue(0);
                entity.Property(e => e.Viewed).HasColumnType("bigint").IsRequired(true).HasDefaultValue(0);
                entity.Property(e => e.ReportsReceivedCount).HasColumnType("bigint").IsRequired(true).HasDefaultValue(0);
                entity.Property(e => e.EditedCount).HasColumnType("bigint").IsRequired(true).HasDefaultValue(0);
                entity.Property(e => e.MediaCount).HasColumnType("bigint").IsRequired(true).HasDefaultValue(0);
            });

            builder.Entity<PostEntity>(entity =>
            {
                entity.Property(e => e.UpdatedAt).IsRequired(false);
                entity.Property(u => u.Id).HasColumnType("bigint"); 
                entity.Property(e => e.Content).HasColumnType("text"); 
                entity.Property(e => e.Title).HasMaxLength(350).IsRequired();
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.ReadTimes).IsRequired().HasMaxLength(120);
                entity.Property(e => e.IsActived).IsRequired().HasDefaultValue(true);
                entity.Property(e => e.EngagementScore).HasDefaultValue(0.0).IsRequired();
            });
            
            builder.Entity<FavoritePostEntity>(entity =>
            {
                entity.Property(f => f.Id).HasColumnType("bigint");
                entity.Property(f => f.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();
            });

            builder.Entity<ReactionPostEntity>(entity => 
            {
                entity.HasKey(r => r.Id);
                entity.Property(u => u.Id).HasColumnType("bigint");
                entity.Property(rp => rp.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();
                entity.HasIndex(rp => new { rp.ApplicationUserId, rp.PostId }).IsUnique();
                entity.Property(rp => rp.Reaction).IsRequired();
            });

            builder.Entity<CommentMetricEntity>(entity => 
            {
                entity.HasKey(c => c.CommentId);
                entity.HasIndex(c => c.ReportCount);
                entity.Property(c => c.CommentId).HasColumnType("bigint");
                entity.Property(c => c.Likes).HasColumnType("bigint").IsRequired().HasDefaultValue(0);
                entity.Property(c => c.DisLikes).HasColumnType("bigint").IsRequired().HasDefaultValue(0);
                entity.Property(c => c.ReportCount).HasColumnType("bigint").IsRequired().HasDefaultValue(0);
                entity.Property(c => c.EditedTimes).HasColumnType("bigint").IsRequired().HasDefaultValue(0);
                entity.Property(c => c.FavoritesCount).HasColumnType("bigint").IsRequired().HasDefaultValue(0);
                entity.Property(c => c.RepliesCount).HasColumnType("bigint").IsRequired().HasDefaultValue(0);
                entity.Property(c => c.ViewsCount).HasColumnType("bigint").IsRequired().HasDefaultValue(0);
                entity.Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();
                entity.Property(c => c.UpdatedAt).IsRequired(false);
            });

            builder.Entity<ReactionCommentEntity>(entity => 
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();
                entity.Property(f => f.CommentId).HasColumnType("bigint");
                entity.Property(f => f.Id).HasColumnType("bigint");
                entity.Property(f => f.Reaction).IsRequired();
                entity.HasIndex(rc => new { rc.ApplicationUserId, rc.CommentId }).IsUnique();
                entity.Property(e => e.UpdatedAt).IsRequired(false);
            });

            builder.Entity<FavoriteCommentEntity>(entity => 
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();
                entity.Property(f => f.CommentId).HasColumnType("bigint");
                entity.Property(f => f.Id).HasColumnType("bigint");
            });

            builder.Entity<CommentEntity>(entity =>
            {
                entity.Property(c => c.Id).HasColumnType("bigint");
                entity.Property(c => c.ParentId).HasColumnType("bigint");

                entity.HasOne(c => c.ParentComment)
                      .WithMany(c => c.Replies)
                      .HasForeignKey(c => c.ParentId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();
                entity.Property(e => e.UpdatedAt).IsRequired(false);
                entity.Property(e => e.Content).IsRequired().HasMaxLength(800);
            });

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.NotificationEntities)
                .WithOne(n => n.ApplicationUser)
                .HasForeignKey(n => n.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<NotificationEntity>(entity => 
            {
                entity.HasKey(n => n.Id);
                entity.Property(n => n.Title).IsRequired().HasMaxLength(200);
                entity.Property(n => n.Content).IsRequired().HasMaxLength(1000);
                entity.Property(n => n.LinkUrl).IsRequired(false).HasMaxLength(2000);
                entity.Property(n => n.IconCssClass).IsRequired(false).HasMaxLength(200);
                entity.Property(n => n.NotificationType).IsRequired();
                entity.Property(e => e.IsRead).IsRequired().HasDefaultValue(false);
                entity.Property(e => e.RelatedEntityId).IsRequired(false);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();
                entity.Property(e => e.UpdatedAt).IsRequired(false);
            });

            builder.Entity<ApplicationUser>().ToTable("app_users");
            builder.Entity<IdentityRole>().ToTable("app_roles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("app_user_claims");
            builder.Entity<IdentityUserRole<string>>().ToTable("app_user_roles");
            builder.Entity<IdentityUserLogin<string>>().ToTable("app_user_logins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("app_role_claims");
            builder.Entity<IdentityUserToken<string>>().ToTable("app_user_tokens");
        }
    }
}