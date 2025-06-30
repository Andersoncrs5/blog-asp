using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.utils.Filters.FiltersDTO
{
    public class PostFilterDTO
    {
        public string? Title { get; set; } = string.Empty;
        public string? ApplicationUserId { get; set; } = string.Empty;
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public long? CategoryId { get; set; }
        public long? ReadTimesAfter { get; set; }
        public long? ReadTimesBefore { get; set; }
        public double? EngagementScoreAfter { get; set; }
        public double? EngagementScoreBefore { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool IncludeMetric { get; set; } = false;
        public bool IsActived { get; set; } = true;
        public ulong? LikesAfter { get; set; }
        public ulong? LikesBefore { get; set; }
        public ulong? DisLikesAfter { get; set; }
        public ulong? DisLikesBefore { get; set; }
        public ulong? CommentCountAfter { get; set; }
        public ulong? CommentCountBefore { get; set; }
        public ulong? FavoriteCountAfter { get; set; }
        public ulong? FavoriteCountBefore { get; set; }
        public ulong? MediaCountAfter { get; set; }
        public ulong? MediaCountBefore { get; set; }





        


    }
}