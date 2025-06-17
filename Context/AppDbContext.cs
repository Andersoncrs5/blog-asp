using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Blog.entities;
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .HasOne(u => u.UserMetric)
                .WithOne()
                .HasForeignKey<UserMetricEntity>(u => u.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CommentEntity>()
                .HasOne(c => c.CommentMetric)
                .WithOne()
                .HasForeignKey<CommentMetricEntity>(c => c.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.FavoriteCommentEntities)
                .WithOne(fc => fc.ApplicationUser)
                .HasForeignKey(f => f.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CommentEntity>()
                .HasMany(c => c.FavoriteCommentEntities)
                .WithOne(fc => fc.Comment)
                .HasForeignKey(f => f.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserMetricEntity>()
                .HasKey(u => u.ApplicationUserId);

            builder.Entity<UserMetricEntity>(entity =>
            {
               entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.UpdatedAt)
                    .IsRequired(false);
            });

            builder.Entity<UserMetricEntity>()
                .HasIndex(um => um.ProfileViews);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Categories)
                .WithOne() 
                .HasForeignKey(c => c.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Posts)
                .WithOne()
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
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CategoryEntity>()
                .HasMany(p => p.Posts)
                .WithOne() 
                .HasForeignKey(u => u.categoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CategoryEntity>(entity =>
            {
                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP")
                      .ValueGeneratedOnAdd();

                entity.Property(u => u.RowVersion)
                      .IsRowVersion();

                entity.Property(e => e.UpdatedAt)
                      .IsRequired(false);

                entity.HasIndex(e => e.Name).IsUnique();
            });

            builder.Entity<PostMetricEntity>()
                .HasKey(u => u.PostId);

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
                entity.Property(p => p.RowVersion).IsRowVersion();
                entity.Property(p => p.UpdatedAt).IsRequired(false);
                entity.Property(p => p.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();
            });

            builder.Entity<PostEntity>(entity =>
            {
                entity.Property(u => u.RowVersion).IsRowVersion();

                entity.Property(e => e.UpdatedAt).IsRequired(false);
                entity.Property(u => u.Id).HasColumnType("bigint"); 
                entity.Property(e => e.Content).HasColumnType("text"); 
                entity.Property(e => e.Title).HasColumnType("varchar(300)"); 
            });

            builder.Entity<FavoritePostEntity>(entity =>
            {
                entity.Property(f => f.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();
            });

            builder.Entity<CommentMetricEntity>(entity => 
            {
                entity.Property(c => c.CommentId).HasColumnType("bigint");
                entity.HasKey(c => c.CommentId);
                entity.Property(c => c.Likes).HasColumnType("bigint");
                entity.Property(c => c.DisLikes).HasColumnType("bigint");
                entity.Property(c => c.ReportCount).HasColumnType("bigint");
                entity.Property(c => c.EditedTimes).HasColumnType("bigint");
                entity.Property(c => c.FavoritesCount).HasColumnType("bigint");
                entity.Property(c => c.RepliesCount).HasColumnType("bigint");
                entity.Property(c => c.ViewsCount).HasColumnType("bigint");
                entity.Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();
                entity.Property(c => c.UpdatedAt).IsRequired(false);
                entity.Property(c => c.RowVersion).IsRowVersion();
            });

            builder.Entity<FavoriteCommentEntity>(entity => 
            {
                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP")
                      .ValueGeneratedOnAdd();

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

                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.UpdatedAt)
                      .IsRequired(false);

                entity.Property(e => e.RowVersion)
                      .IsRowVersion();

                entity.Property(e => e.Content)
                      .IsRequired()
                      .HasMaxLength(2000);
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