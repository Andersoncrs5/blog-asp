using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.DTOs.User
{
    public class TokenDto
    {
        public string? AcessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}