using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blog.entities;
using blog.utils.Filters.FiltersDTO;
using Microsoft.EntityFrameworkCore;

namespace blog.utils.Filters.FiltersQuerys
{
    public static class NotificationQueryFilter
    {
        public static IQueryable<NotificationEntity> ApplyFilters(
            this IQueryable<NotificationEntity> query,
            NotificationFilter filter
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

            if (!string.IsNullOrWhiteSpace(filter.UserName))
            {
                query = query.Include(f => f.ApplicationUser).Where(c => EF.Functions.Like(c.ApplicationUser.UserName, $"%{filter.UserName}%"));
            }

            if (!string.IsNullOrWhiteSpace(filter.Title))
            {
                query = query.Where(c => EF.Functions.Like(c.Title, $"%{filter.Title}%"));
            }

            if (filter.NotificationType.HasValue)
            {
                query = query.Where(m => m.NotificationType == filter.NotificationType);
            }

            if (filter.IsRead.HasValue)
            {
                query = query.Where(m => m.IsRead == filter.IsRead);
            }

            return query;
        }
    }
}