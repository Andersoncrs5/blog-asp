using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blog.utils.Filters.FiltersDTO;
using Blog.entities;
using Microsoft.EntityFrameworkCore;

namespace blog.utils.Filters.FiltersQuerys
{
    public static class CommentQueryFilter
    {
        public static IQueryable<CommentEntity> ApplyFilters(this IQueryable<CommentEntity> query,CommentFilterDTO filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.Content))
            {
                query = query.Where(c => EF.Functions.Like(c.Content, $"%{filter.Content}%", "%"));
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

            if (filter.IncludeRelationsUser == true)
            {
                query = query.Include(c => c.ApplicationUser);
            }

            if (filter.IncludeRelationsUser == true && !string.IsNullOrWhiteSpace(filter.NameUser) )
            {
                query = query.Where(c => c.ApplicationUser != null && c.ApplicationUser.UserName == filter.NameUser);
            }

            if (filter.PostId.HasValue)
            {
                query = query.Where(c => c.PostId == filter.PostId);
            }

            if (filter.IncludeRelationsMetric == true)
            {
                query = query.Include(c => c.CommentMetric);
            }

            if (filter.LikesAfter.HasValue && filter.IncludeRelationsMetric == true)
            {
                query = query.Where(c => c.CommentMetric != null && c.CommentMetric.Likes >= filter.LikesAfter);
            }

            if (filter.LikesBefore.HasValue && filter.IncludeRelationsMetric == true)
            {
                query = query.Where(c => c.CommentMetric != null && c.CommentMetric.Likes <= filter.LikesBefore);
            }

            if (filter.DisLikesAfter.HasValue && filter.IncludeRelationsMetric == true)
            {
                query = query.Where(c => c.CommentMetric != null && c.CommentMetric.DisLikes >= filter.DisLikesAfter);
            }

            if (filter.DisLikesBefore.HasValue && filter.IncludeRelationsMetric == true)
            {
                query = query.Where(c => c.CommentMetric != null && c.CommentMetric.DisLikes <= filter.DisLikesBefore);
            }

            if (filter.FavoritesCountAfter.HasValue && filter.IncludeRelationsMetric == true)
            {
                query = query.Where(c => c.CommentMetric != null && c.CommentMetric.FavoritesCount >= filter.FavoritesCountAfter);
            }

            if (filter.FavoritesCountBefore.HasValue && filter.IncludeRelationsMetric == true)
            {
                query = query.Where(c => c.CommentMetric != null && c.CommentMetric.FavoritesCount <= filter.FavoritesCountBefore);
            }

            if (filter.RepliesCountAfter.HasValue && filter.IncludeRelationsMetric == true)
            {
                query = query.Where(c => c.CommentMetric != null && c.CommentMetric.RepliesCount >= filter.RepliesCountAfter);
            }

            if (filter.RepliesCountBefore.HasValue && filter.IncludeRelationsMetric == true)
            {
                query = query.Where(c => c.CommentMetric != null && c.CommentMetric.RepliesCount <= filter.RepliesCountBefore);
            }

            if (filter.ViewsCountAfter.HasValue && filter.IncludeRelationsMetric == true)
            {
                query = query.Where(c => c.CommentMetric != null && c.CommentMetric.ViewsCount >= filter.ViewsCountAfter);
            }

            if (filter.ViewsCountBefore.HasValue && filter.IncludeRelationsMetric == true)
            {
                query = query.Where(c => c.CommentMetric != null && c.CommentMetric.ViewsCount <= filter.ViewsCountBefore);
            }

            return query;
        }
        
    }
}