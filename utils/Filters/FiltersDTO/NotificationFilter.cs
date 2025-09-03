using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blog.entities.enums;

namespace blog.utils.Filters.FiltersDTO
{
    public class NotificationFilter : PageModel
    {
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public string Title { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public NotificationTypeEnum? NotificationType { get; set; }
        public bool? IsRead { get; set; }

    }
}