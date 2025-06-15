using System;
using System.Collections.Generic;
using System.Linq;
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
                .HasForeignKey<UserMetricEntity>(u => u.ApplicationUserId);

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

            builder.Entity<UserMetricEntity>()
                .Property(u => u.RowVersion)
                .IsRowVersion();

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