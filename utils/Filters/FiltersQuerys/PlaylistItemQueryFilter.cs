using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blog.utils.Filters.FiltersDTO;
using Blog.entities;

namespace blog.utils.Filters.FiltersQuerys
{
    public static class PlaylistItemQueryFilter
    {
        public static IQueryable<PlaylistItemEntity> ApplyFilters(
            this IQueryable<PlaylistItemEntity> query,
            PlaylistItemFilter filter
        )
        {
            if (filter.CreatedAfter.HasValue)
            {
                query = query.Where(c => c.CreatedAt.ToUniversalTime() >= filter.CreatedAfter.Value.ToUniversalTime());
            }

            if (filter.CreatedBefore.HasValue)
            {
                query = query.Where(c => c.CreatedAt.ToUniversalTime() <= filter.CreatedBefore.Value.ToUniversalTime());
            }

            if (filter.Order.HasValue)
            {
                query = query.Where(c => c.Order == filter.Order);
            }

            return query;
        }
    }
}