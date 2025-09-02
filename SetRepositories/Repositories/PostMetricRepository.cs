using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Context;
using Blog.entities;
using Blog.entities.enums;
using Blog.SetRepositories.IRepositories;
using Blog.utils;
using Blog.utils.enums;
using Microsoft.EntityFrameworkCore;

namespace Blog.SetRepositories.Repositories
{
    public class PostMetricRepository: IPostMetricRepository
    {
        private readonly AppDbContext _context;

        public PostMetricRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PostMetricEntity> Create(PostEntity post)
        {
            PostMetricEntity metric = new PostMetricEntity
            {
                PostId = post.Id
            };

            var result = await _context.PostMetricEntities.AddAsync(metric);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<PostMetricEntity?> Get(PostEntity post)
        {
            PostMetricEntity? metric = await _context.PostMetricEntities.
                AsNoTracking().FirstOrDefaultAsync(u => u.PostId == post.Id);

            if (metric == null)
                return null;

            return metric;
        }
    
        public async Task<PostMetricEntity> SumOrRedLikeOrDislike(PostMetricEntity metric, SumOrRedEnum action, LikeOrDislike ld)
        {
            if (action == SumOrRedEnum.SUM && ld == LikeOrDislike.LIKE ) {
                metric.Likes += 1;
            }

            if (action == SumOrRedEnum.REDUCE && ld == LikeOrDislike.LIKE ) {
                metric.Likes -= 1;
            }

            if (action == SumOrRedEnum.SUM && ld == LikeOrDislike.DISLIKE ) {
                metric.DisLikes += 1;
            }

            if (action == SumOrRedEnum.REDUCE && ld == LikeOrDislike.DISLIKE ) {
                metric.Likes -= 1;
            }

            metric.UpdatedAt = DateTime.UtcNow;

            _context.Entry(metric).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return metric;
        }
    
        public async Task<PostMetricEntity> SumOrRedShares(PostMetricEntity metric, SumOrRedEnum action) 
        {
            if (action == SumOrRedEnum.SUM) 
            {
                metric.Shares += 1;
            }

            if (action == SumOrRedEnum.REDUCE) 
            {
                metric.Shares -= 1;
            }

            _context.Entry(metric).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return metric;
        }

        public async Task<PostMetricEntity> SumOrRedCommentCount(PostMetricEntity metric, SumOrRedEnum action)
        {
            if (action == SumOrRedEnum.SUM) 
            {
                metric.CommentCount += 1;
            }

            if (action == SumOrRedEnum.REDUCE) 
            {
                metric.CommentCount -= 1;
            }

            _context.Entry(metric).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return metric;
        }

        public async Task<PostMetricEntity> SumOrRedFavoriteCount(PostMetricEntity metric, SumOrRedEnum action)
        {
            if (action == SumOrRedEnum.SUM) 
            {
                metric.FavoriteCount += 1;
            }

            if (action == SumOrRedEnum.REDUCE) 
            {
                metric.FavoriteCount -= 1;
            }

            _context.Entry(metric).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return metric;
        }
        
        public async Task<PostMetricEntity> SumOrRedBookmarks(PostMetricEntity metric, SumOrRedEnum action)
        {
            if (action == SumOrRedEnum.SUM) 
            {
                metric.Bookmarks += 1;
            }

            if (action == SumOrRedEnum.REDUCE) 
            {
                metric.Bookmarks -= 1;
            }

            _context.Entry(metric).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return metric;
        }

        public async Task<PostMetricEntity> SumOrRedViewed(PostMetricEntity metric, SumOrRedEnum action)
        {
            if (action == SumOrRedEnum.SUM) 
            {
                metric.Viewed += 1;
            }

            if (action == SumOrRedEnum.REDUCE) 
            {
                metric.Viewed -= 1;
            }

            _context.Entry(metric).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return metric;
        }

        public async Task<PostMetricEntity> SumOrRedReportsReceivedCount(PostMetricEntity metric, SumOrRedEnum action)
        {
            if (action == SumOrRedEnum.SUM) 
            {
                metric.ReportsReceivedCount += 1;
            }

            if (action == SumOrRedEnum.REDUCE) 
            {
                metric.ReportsReceivedCount -= 1;
            }

            _context.Entry(metric).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return metric;
        }

        public async Task<PostMetricEntity> SumOrRedEditedCount(PostMetricEntity metric, SumOrRedEnum action)
        {
            if (action == SumOrRedEnum.SUM) 
            {
                metric.EditedCount += 1;
            }

            if (action == SumOrRedEnum.REDUCE) 
            {
                metric.EditedCount -= 1;
            }

            _context.Entry(metric).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return metric;
        }

        public async Task<PostMetricEntity> SumOrRedMediaCount(PostMetricEntity metric, SumOrRedEnum action) 
        {
            if (action == SumOrRedEnum.SUM) 
            {
                metric.MediaCount += 1;
            }

            if (action == SumOrRedEnum.REDUCE) 
            {
                metric.MediaCount -= 1;
            }

            _context.Entry(metric).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return metric;
        }

    }
}