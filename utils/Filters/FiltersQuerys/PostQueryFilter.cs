using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.entities;
using Blog.utils.Filters.FiltersDTO;
using Microsoft.EntityFrameworkCore;

namespace Blog.utils.Filters.FiltersQuerys
{
    public static class PostQueryFilter
    {
        public static IQueryable<PostEntity> ApplyFilters(
            this IQueryable<PostEntity> query,
            PostFilterDTO filter
        )
        {
            if (!string.IsNullOrWhiteSpace(filter.Title))
            {
                query = query.Where(c => c.Title.Contains(filter.Title));
            }

            if (!string.IsNullOrWhiteSpace(filter.ApplicationUserId))
            {
                query = query.Where(c => c.ApplicationUserId == filter.ApplicationUserId);
            }

            if (filter.CreatedAfter.HasValue)
            {
                query = query.Where(c => c.CreatedAt.ToUniversalTime() >= filter.CreatedAfter.Value.ToUniversalTime());
            }

            if (filter.CreatedBefore.HasValue)
            {
                query = query.Where(c => c.CreatedAt.ToUniversalTime() <= filter.CreatedBefore.Value.ToUniversalTime());
            }

            if (filter.CategoryId.HasValue)
            {
                query = query.Where(c => c.CategoryId == filter.CategoryId);
            }

            if (filter.ReadTimesAfter.HasValue)
            {
                query = query.Where(c => c.ReadTimes >= filter.ReadTimesAfter.Value);
            }

            if (filter.ReadTimesBefore.HasValue)
            {
                query = query.Where(c => c.ReadTimes <= filter.ReadTimesBefore.Value);
            }

            if (filter.EngagementScoreAfter.HasValue)
            {
                query = query.Where(c => c.EngagementScore >= filter.EngagementScoreAfter.Value);
            }

            if (filter.EngagementScoreBefore.HasValue)
            {
                query = query.Where(c => c.EngagementScore <= filter.EngagementScoreBefore.Value);
            }

            if (filter.IncludeMetric == true){ query = query.Include(p => p.PostMetricEntity); }

            if (filter.LikesAfter.HasValue && filter.IncludeMetric == true )
            {
                query = query.Where(m => m.PostMetricEntity != null && m.PostMetricEntity.Likes >= filter.LikesAfter.Value);
            }

            if (filter.LikesBefore.HasValue && filter.IncludeMetric == true )
            {
                query = query.Where(m => m.PostMetricEntity != null && m.PostMetricEntity.Likes <= filter.LikesBefore.Value);
            }

            if (filter.DisLikesAfter.HasValue && filter.IncludeMetric == true )
            {
                query = query.Where(m => m.PostMetricEntity != null && m.PostMetricEntity.DisLikes >= filter.DisLikesAfter.Value);
            }

            if (filter.DisLikesBefore.HasValue && filter.IncludeMetric == true )
            {
                query = query.Where(m => m.PostMetricEntity != null && m.PostMetricEntity.DisLikes <= filter.DisLikesBefore.Value);
            }

            if (filter.CommentCountAfter.HasValue && filter.IncludeMetric == true )
            {
                query = query.Where(m => m.PostMetricEntity != null && m.PostMetricEntity.CommentCount >= filter.CommentCountAfter.Value);
            }

            if (filter.CommentCountBefore.HasValue && filter.IncludeMetric == true )
            {
                query = query.Where(m => m.PostMetricEntity != null && m.PostMetricEntity.CommentCount <= filter.CommentCountBefore.Value);
            }

            if (filter.FavoriteCountAfter.HasValue && filter.IncludeMetric == true )
            {
                query = query.Where(m => m.PostMetricEntity != null && m.PostMetricEntity.FavoriteCount >= filter.FavoriteCountAfter.Value);
            }

            if (filter.FavoriteCountBefore.HasValue && filter.IncludeMetric == true )
            {
                query = query.Where(m => m.PostMetricEntity != null && m.PostMetricEntity.FavoriteCount <= filter.FavoriteCountBefore.Value);
            }

            if (filter.MediaCountAfter.HasValue && filter.IncludeMetric == true )
            {
                query = query.Where(m => m.PostMetricEntity != null && m.PostMetricEntity.MediaCount >= filter.MediaCountAfter.Value);
            }

            if (filter.MediaCountBefore.HasValue && filter.IncludeMetric == true )
            {
                query = query.Where(m => m.PostMetricEntity != null && m.PostMetricEntity.MediaCount <= filter.MediaCountBefore.Value);
            }

            return query;
        }

        public static IQueryable<PostEntity> ApplySorting(
            this IQueryable<PostEntity> query, string? sortBy, bool ascending)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return query.OrderBy(c => c.Id);
            }

            switch (sortBy.ToLowerInvariant())
            {
                case "name":
                    query = ascending ? query.OrderBy(c => c.Title) : query.OrderByDescending(c => c.Title);
                    break;
                case "createdat":
                    query = ascending ? query.OrderBy(c => c.CreatedAt) : query.OrderByDescending(c => c.CreatedAt);
                    break;
                
                default:
                    query = query.OrderBy(c => c.Id);
                    break;
            }
            return query;
        }

    }
}