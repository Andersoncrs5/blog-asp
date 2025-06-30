using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blog.utils.Filters.FiltersDTO;
using Blog.entities;
using Microsoft.EntityFrameworkCore;

namespace blog.utils.Filters.FiltersQuerys
{
    public static class PlaylistQueryFilter
    {
        public static IQueryable<PlaylistEntity> ApplyFilters(
            this IQueryable<PlaylistEntity> query,
            PlaylistFilterDTO filter
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

            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                query = query.Where(c => EF.Functions.Like(c.Name, $"%{filter.Name}%", "%"));
            }

            if (filter.IsPublic.HasValue)
            {
                query = query.Where(c => c.IsPublic == filter.IsPublic);
            }

            if (filter.ItemCountAfter.HasValue)
            {
                query = query.Where(c => c.ItemCount >= filter.ItemCountAfter);
            }

            if (filter.ItemCountBefore.HasValue)
            {
                query = query.Where(c => c.ItemCount <= filter.ItemCountBefore);
            }

            return query;
        }
    }
}