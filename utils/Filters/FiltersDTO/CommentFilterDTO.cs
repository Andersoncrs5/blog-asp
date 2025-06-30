using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blog.utils.Filters.FiltersDTO
{
    public class CommentFilterDTO
    {
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public string? Content { get; set; } = string.Empty;
        public string? ApplicationUserId { get; set; } = string.Empty;
        public string? NameUser { get; set; } = string.Empty;
        public long? PostId { get; set; }
        public ulong? LikesAfter { get; set; }
        public ulong? LikesBefore { get; set; }
        public ulong? DisLikesAfter { get; set; }
        public ulong? DisLikesBefore { get; set; }
        public ulong? FavoritesCountAfter { get; set; }
        public ulong? FavoritesCountBefore { get; set; }
        public ulong? RepliesCountAfter { get; set; }
        public ulong? RepliesCountBefore { get; set; }
        public ulong? ViewsCountAfter { get; set; }
        public ulong? ViewsCountBefore { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        
        public bool IncludeRelationsUser { get; set; } = false;
        public bool IncludeRelationsMetric { get; set; } = false;
        
    }
}