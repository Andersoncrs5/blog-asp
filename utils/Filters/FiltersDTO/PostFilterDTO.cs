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
    }
}