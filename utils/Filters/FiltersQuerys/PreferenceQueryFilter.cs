using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blog.entities;
using blog.utils.Filters.FiltersDTO;
using Microsoft.EntityFrameworkCore;

namespace blog.utils.Filters.FiltersQuerys
{
    public static class PreferenceQueryFilter
    {
        public static IQueryable<UserPreferenceEntity> ApplyFilters(
            this IQueryable<UserPreferenceEntity> query,
            PreferenceFilterDTO filter
        )
        {
            if (!string.IsNullOrWhiteSpace(filter.ApplicationUserId))
            {
                query = query.Where(c => c.ApplicationUserId == filter.ApplicationUserId);
            }

            if (filter.CategoryId.HasValue)
            {
                query = query.Where(c => c.CategoryId == filter.CategoryId);
            }

            if (filter.CreatedAfter.HasValue)
            {
                query = query.Where(c => c.CreatedAt.ToUniversalTime() >= filter.CreatedAfter.Value.ToUniversalTime());
            }

            if (filter.CreatedBefore.HasValue)
            {
                query = query.Where(c => c.CreatedAt.ToUniversalTime() <= filter.CreatedBefore.Value.ToUniversalTime());
            }

            if (filter.IncludeRelations.HasValue)
            {
                query = query.Include(u => u.Category);
            }

            return query;
        }
    }
}