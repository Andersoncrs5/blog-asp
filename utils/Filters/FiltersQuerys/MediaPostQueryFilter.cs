using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blog.utils.Filters.FiltersDTO;
using Blog.entities;

namespace blog.utils.Filters.FiltersQuerys
{
    public static class MediaPostQueryFilter
    {
        public static IQueryable<MediaPostEntity> ApplyFilters(
            this IQueryable<MediaPostEntity> query,
            MediaPostFilter filter
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

            if (filter.PostId.HasValue)
            {
                query = query.Where(m => m.PostId == filter.PostId);
            }

            if (filter.MediaType.HasValue)
            {
                query = query.Where(m => m.MediaType == filter.MediaType);
            }

            if (filter.Order.HasValue)
            {
                query = query.Where(m => m.Order == filter.Order);
            }

            return query;
        }
    }
}