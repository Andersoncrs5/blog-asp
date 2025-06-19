using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.utils.Responses
{
    public class ResponseTokens
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiredAt { get; set; }
        public int StatusCode  { get; set; }

        public ResponseTokens() { }

        public ResponseTokens(string token, string refresh, DateTime exp, int statusCode)
        {
            Token = token;
            RefreshToken = refresh;
            ExpiredAt = exp;
            StatusCode = statusCode;
        }
    }
}