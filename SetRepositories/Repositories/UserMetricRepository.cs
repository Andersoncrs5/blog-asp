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
    public class UserMetricRepository: IUserMetricRepository
    {
        private readonly AppDbContext _context;

        public UserMetricRepository(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task<UserMetricEntity> Get(string? userId) 
        {
            if(string.IsNullOrWhiteSpace(userId))
                throw new ResponseException("User id is required", 400, "fail");

            UserMetricEntity? metric = await this._context.UserMetrics.AsNoTracking()
            .FirstOrDefaultAsync(u => u.ApplicationUserId == userId);

            if (metric == null)
                throw new ResponseException("User metric not found", 404, "fail");

            return metric;
        }

        public async Task<UserMetricEntity> Create(string userId) 
        {
            if(string.IsNullOrWhiteSpace(userId))
                throw new ResponseException("User id is required", 400, "fail");

            UserMetricEntity metric = new UserMetricEntity();
            metric.ApplicationUserId = userId;

            var metricCreated = await this._context.UserMetrics.AddAsync(metric);

            return metricCreated.Entity;
        }

        public async Task<UserMetricEntity> SumOrRedLikesOrDislikeGivenCountInComment(UserMetricEntity metric, SumOrRedEnum action, LikeOrDislike l) 
        {
            if (action == SumOrRedEnum.SUM && l == LikeOrDislike.LIKE ) {
                metric.LikesGivenCountInComment += 1;
            }

            if (action == SumOrRedEnum.REDUCE && l == LikeOrDislike.LIKE ) {
                metric.LikesGivenCountInComment -= 1;
            }

            if (action == SumOrRedEnum.SUM && l == LikeOrDislike.DISLIKE ) {
                metric.DeslikesGivenCountInComment += 1;
            }

            if (action == SumOrRedEnum.REDUCE && l == LikeOrDislike.DISLIKE ) {
                metric.DeslikesGivenCountInComment -= 1;
            }

            _context.Entry(metric).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return metric;
        }

        //
        public async Task<UserMetricEntity> SumOrRedLikesOrDislikeGivenCountInPost(UserMetricEntity metric, SumOrRedEnum action, LikeOrDislike l) // NOVO MÉTODO
        {
            if (action == SumOrRedEnum.SUM && l == LikeOrDislike.LIKE)
            {
                metric.LikesGivenCountInPost += 1;
            }

            if (action == SumOrRedEnum.REDUCE && l == LikeOrDislike.LIKE)
            {
                metric.LikesGivenCountInPost -= 1;
            }

            if (action == SumOrRedEnum.SUM && l == LikeOrDislike.DISLIKE)
            {
                metric.DeslikesGivenCountInPost += 1;
            }

            if (action == SumOrRedEnum.REDUCE && l == LikeOrDislike.DISLIKE)
            {
                metric.DeslikesGivenCountInPost -= 1;
            }

            _context.Entry(metric).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return metric;
        }

        //
        public async Task<UserMetricEntity> SumOrRedFollowersCount(UserMetricEntity metric, SumOrRedEnum action) 
        {
            if (action == SumOrRedEnum.SUM) 
            {
                metric.FollowersCount += 1;
            }

            if (action == SumOrRedEnum.REDUCE) 
            {
                metric.FollowersCount -= 1;
            }

            _context.Entry(metric).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return metric;
        }

        //
        public async Task<UserMetricEntity> SumOrRedFollowingCount(UserMetricEntity metric, SumOrRedEnum action) 
        {
            if (action == SumOrRedEnum.SUM) 
            {
                metric.FollowingCount += 1;
            }

            if (action == SumOrRedEnum.REDUCE) 
            {
                metric.FollowingCount -= 1;
            }

            _context.Entry(metric).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return metric;
        }

        public async Task<UserMetricEntity> SumOrRedPostsCount(UserMetricEntity metric, SumOrRedEnum action) 
        {
            if (action == SumOrRedEnum.SUM) 
            {
                metric.PostsCount += 1;
            }

            if (action == SumOrRedEnum.REDUCE) 
            {
                metric.PostsCount -= 1;
            }

            _context.Entry(metric).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return metric;
        }

        public async Task<UserMetricEntity> SumOrRedCommentsCount(UserMetricEntity metric, SumOrRedEnum action) 
        {
            if (action == SumOrRedEnum.SUM) 
            {
                metric.CommentsCount += 1;
            }

            if (action == SumOrRedEnum.REDUCE) 
            {
                metric.CommentsCount -= 1;
            }

            _context.Entry(metric).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return metric;
        }

        //
        public async Task<UserMetricEntity> SumOrRedSharesCount(UserMetricEntity metric, SumOrRedEnum action) 
        {
            if (action == SumOrRedEnum.SUM) 
            {
                metric.SharesCount += 1;
            }

            if (action == SumOrRedEnum.REDUCE) 
            {
                metric.SharesCount -= 1;
            }

            _context.Entry(metric).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return metric;
        }
    
        //
        public async Task<UserMetricEntity> SumOrRedSavedMediaCount(UserMetricEntity metric, SumOrRedEnum action) 
        {
            if (action == SumOrRedEnum.SUM) 
            {
                metric.SavedMediaCount += 1;
            }

            if (action == SumOrRedEnum.REDUCE) 
            {
                metric.SavedMediaCount -= 1;
            }

            _context.Entry(metric).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return metric;
        }

        //
        public async Task<UserMetricEntity> SumOrRedReportsReceivedCount(UserMetricEntity metric, SumOrRedEnum action)
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

        //
        public async Task<UserMetricEntity> SumOrRedMediaUploadsCount(UserMetricEntity metric, SumOrRedEnum action) 
        {
            if (action == SumOrRedEnum.SUM) 
            {
                metric.MediaUploadsCount += 1;
            }

            if (action == SumOrRedEnum.REDUCE) 
            {
                metric.MediaUploadsCount -= 1;
            }

            _context.Entry(metric).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return metric;
        }

        public async Task<UserMetricEntity> SumOrRedSavedPostsCount(UserMetricEntity metric, SumOrRedEnum action)
        {
            if (action == SumOrRedEnum.SUM) 
            {
                metric.SavedPostsCount += 1;
            }

            if (action == SumOrRedEnum.REDUCE) 
            {
                metric.SavedPostsCount -= 1;
            }

            _context.Entry(metric).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return metric;
        }

        public async Task<UserMetricEntity> SumOrRedSavedCommentsCount(UserMetricEntity metric, SumOrRedEnum action)
        {
            if (action == SumOrRedEnum.SUM) 
            {
                metric.SavedCommentsCount += 1;
            }

            if (action == SumOrRedEnum.REDUCE) 
            {
                metric.SavedCommentsCount -= 1;
            }

            _context.Entry(metric).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return metric;
        }
    
        public async Task<UserMetricEntity> SumOrRedEditedCount(UserMetricEntity metric, SumOrRedEnum action)
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
    
        public async Task<UserMetricEntity> SumOrRedProfileViews(UserMetricEntity metric, SumOrRedEnum action)
        {
            if (action == SumOrRedEnum.SUM) 
            {
                metric.ProfileViews += 1;
            }

            if (action == SumOrRedEnum.REDUCE) 
            {
                metric.ProfileViews -= 1;
            }

            _context.Entry(metric).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return metric;
        }
    
        public async Task<UserMetricEntity> SetLastLogin(UserMetricEntity metric)
        {
            metric.LastLogin = DateTime.UtcNow;

            _context.Entry(metric).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return metric;
        }
    
        public async Task<UserMetricEntity> SumOrRedPlaylistCount(UserMetricEntity metric, SumOrRedEnum action)
        {
            if (action == SumOrRedEnum.SUM) 
            {
                metric.PlaylistCount += 1;
            }

            if (action == SumOrRedEnum.REDUCE) 
            {
                metric.PlaylistCount -= 1;
            }

            _context.Entry(metric).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return metric;
        }
    
        public async Task<UserMetricEntity> SumOrRedPreferenceCount(UserMetricEntity metric, SumOrRedEnum action)
        {
            if (action == SumOrRedEnum.SUM) 
            {
                metric.PreferenceCount += 1;
            }

            if (action == SumOrRedEnum.REDUCE) 
            {
                metric.PreferenceCount -= 1;
            }

            _context.Entry(metric).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return metric;
        }
    

    }
}