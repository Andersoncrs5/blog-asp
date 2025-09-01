using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blog.utils.Responses
{
    public class ResponseBody<T>
    {
        public bool? Status { get; set; }

        public string? Message { get; set; }
        public int? Code { get; set; }
        public T? Body { get; set; }
        public string? Url { get; set; }
        public DateTimeOffset? Datetime { get; set; }

    }
}