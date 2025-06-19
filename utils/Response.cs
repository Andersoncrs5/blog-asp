using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.utils
{
    public class Response
    {
        public string? Status { get; set; }

        public string? Message { get; set; }
        public int? Code { get; set; }
        public object? data { get; set; }
        public string? Url { get; set; }


        public Response(string? status, string? message, int? code, object? data, string? url = null)
        {
            Status = status;
            Message = message;
            Code = code;
            this.data = data;
            Url = url;
        }

    }
}