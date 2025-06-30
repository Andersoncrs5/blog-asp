using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blog.utils.Filters.FiltersDTO
{
    public class PlaylistFilterDTO
    {
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public string? Name { get; set; }
        public bool? IsPublic { get; set; }
        public byte? ItemCountAfter { get; set; }
        public byte? ItemCountBefore { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}