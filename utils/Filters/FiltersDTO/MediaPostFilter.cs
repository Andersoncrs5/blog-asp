using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.entities.enums;

namespace blog.utils.Filters.FiltersDTO
{
    public class MediaPostFilter: PageModel
    {
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public long? PostId { get; set; }
        public MediaTypeEnum? MediaType { get; set; }
        public int? Order { get; set; }
    }
}