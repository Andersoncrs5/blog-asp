using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.utils
{
    public class ResponseException: Exception
    {
        public int StatusCode { get; set; }
        public string Status { get; set; }

        public object? Description { get; set; }

        public ResponseException(string message, int statusCode = 400, string status = "fail", object? description = null) : base(message)
        {
            StatusCode = statusCode;
            Status = status;
            Description = description;
        }
    }
}