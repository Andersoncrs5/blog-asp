using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blog.utils.Filters.FiltersDTO
{
    public class FavoriteCommentFilter: PageModel
    {
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public string? UserName { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}