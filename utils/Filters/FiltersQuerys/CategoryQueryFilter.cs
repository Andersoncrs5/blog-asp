using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blog.utils.Filters.FiltersDTO;
using Blog.entities;
using Microsoft.EntityFrameworkCore;

namespace blog.utils.Filters.FiltersQuerys
{
    public static class CategoryQueryFilter
    {
        public static IQueryable<CategoryEntity> ApplyFilters(this IQueryable<CategoryEntity> query,CategoryFilterDTO filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                query = query.Where(c => EF.Functions.Like(c.Name, $"%{filter.Name}%", "%"));
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

            if (filter.IsActived.HasValue)
            {
                query = query.Where(c => c.IsActived == filter.IsActived);
            }

            return query;
        }
    }
}