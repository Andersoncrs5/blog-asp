using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blog.utils.Filters.FiltersDTO
{
    public class PlaylistItemFilter: PageModel
    {
        public int? Order { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }

    }
}