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
    public class CommentMetricRepository: ICommentMetricRepository
    {
        private readonly AppDbContext _context;

        public CommentMetricRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CommentMetricEntity> Get(CommentEntity comment)
        {
            CommentMetricEntity? metric = await _context.CommentMetricEntities .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CommentId == comment.Id);

            if (metric is null)
                throw new ResponseException("Metric not found", 404);
            
            return metric;
        }
    
        public async Task<CommentMetricEntity> Create(CommentEntity comment)
        {
            CommentMetricEntity metric = new CommentMetricEntity()
            {
                CommentId = comment.Id
            };

            var result = await _context.CommentMetricEntities.AddAsync(metric);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<CommentMetricEntity> SumOrRedLikeOrDislike(CommentMetricEntity metric, SumOrRedEnum action, LikeOrDislike ld)
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

            _context.Entry(metric).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return metric;
        }

        public async Task<CommentMetricEntity> SumOrRedReportCount(CommentMetricEntity metric, SumOrRedEnum action)
        {
            if (action == SumOrRedEnum.SUM) 
            {
                metric.ReportCount += 1;
            }

            if (action == SumOrRedEnum.REDUCE) 
            {
                metric.ReportCount -= 1;
            }

            _context.Entry(metric).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return metric;
        }

        public async Task<CommentMetricEntity> SumOrRedEditedTimes(CommentMetricEntity metric, SumOrRedEnum action)
        {
            if (action == SumOrRedEnum.SUM) 
            {
                metric.EditedTimes += 1;
            }

            if (action == SumOrRedEnum.REDUCE) 
            {
                metric.EditedTimes -= 1;
            }

            _context.Entry(metric).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return metric;
        }

        public async Task<CommentMetricEntity> SumOrRedFavoritesCount(CommentMetricEntity metric, SumOrRedEnum action)
        {
            if (action == SumOrRedEnum.SUM) 
            {
                metric.FavoritesCount += 1;
            }

            if (action == SumOrRedEnum.REDUCE) 
            {
                metric.FavoritesCount -= 1;
            }

            _context.Entry(metric).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return metric;
        }

        public async Task<CommentMetricEntity> SumOrRedRepliesCount(CommentMetricEntity metric, SumOrRedEnum action)
        {
            if (action == SumOrRedEnum.SUM) 
            {
                metric.RepliesCount += 1;
            }

            if (action == SumOrRedEnum.REDUCE) 
            {
                metric.RepliesCount -= 1;
            }

            _context.Entry(metric).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return metric;
        }

        public async Task<CommentMetricEntity> SumOrRedViewsCount(CommentMetricEntity metric, SumOrRedEnum action)
        {
            if (action == SumOrRedEnum.SUM) 
            {
                metric.ViewsCount += 1;
            }

            if (action == SumOrRedEnum.REDUCE) 
            {
                metric.ViewsCount -= 1;
            }

            _context.Entry(metric).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return metric;
        }

    }
}