using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.utils.Responses
{
    public class ResponseTokens
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpiredAt { get; set; }
        public DateTime? ExpiredAtRefreshToken { get; set; }
    }
}