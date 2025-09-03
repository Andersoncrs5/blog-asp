using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blog.utils.Filters.FiltersDTO;
using Blog.entities;
using Microsoft.EntityFrameworkCore;

namespace blog.utils.Filters.FiltersQuerys
{
    public static class FavoriteCommentQueryFilter
    {
        public static IQueryable<FavoriteCommentEntity> ApplyFilters(
            this IQueryable<FavoriteCommentEntity> query,
            FavoriteCommentFilter filter
        )
        {
            if (filter.CreatedAfter.HasValue)
            {
                query = query.Where(c => c.CreatedAt >= filter.CreatedAfter.Value);
            }

            if (filter.CreatedBefore.HasValue)
            {
                query = query.Where(c => c.CreatedAt <= filter.CreatedBefore.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.UserName) || !string.IsNullOrWhiteSpace(filter.Content))
            {
                query = query.Include(f => f.ApplicationUser).Include(f => f.Comment);
            }

            if (!string.IsNullOrWhiteSpace(filter.UserName))
            {
                query = query.Where(c => EF.Functions.Like(c.ApplicationUser.UserName, $"%{filter.UserName}%"));
            }

            if (!string.IsNullOrWhiteSpace(filter.Content))
            {
                query = query.Where(c => EF.Functions.Like(c.Comment.Content, $"%{filter.Content}%"));
            }

            return query;
        }
    }

}