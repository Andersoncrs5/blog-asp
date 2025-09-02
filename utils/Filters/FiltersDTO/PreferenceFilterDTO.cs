using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blog.utils.Filters.FiltersDTO
{
    public class PreferenceFilterDTO: PageModel
    {
        public string ApplicationUserId { get; set; } = string.Empty;
        public long? CategoryId { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public bool? IncludeRelations { get; set; }
    }
}