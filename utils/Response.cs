using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.utils
{
    public class Response<T>
    {
        public string? Status { get; set; }
        public string? Message { get; set; }
        public int? Code { get; set; }
        public T? data { get; set; }
    }
}