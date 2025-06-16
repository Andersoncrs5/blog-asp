using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.entities;
using Blog.utils.Filters.FiltersDTO;

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

            return query;
        }

        public static IQueryable<CategoryEntity> ApplyPagination(
            this IQueryable<CategoryEntity> query, int pageNumber, int pageSize)
        {
            return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        public static IQueryable<CategoryEntity> ApplySorting(
            this IQueryable<CategoryEntity> query, string? sortBy, bool ascending)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return query.OrderBy(c => c.Id);
            }

            switch (sortBy.ToLowerInvariant())
            {
                case "name":
                    query = ascending ? query.OrderBy(c => c.Name) : query.OrderByDescending(c => c.Name);
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