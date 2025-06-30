using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blog.utils.Filters.FiltersDTO
{
    public class CategoryFilterDTO
    {
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public string? ApplicationUserId { get; set; }
        public bool? IsActived { get; set; }
        public string? Name { get; set; }
    }
}