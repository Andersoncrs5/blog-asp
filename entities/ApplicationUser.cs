using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Blog.entities
{
    public class ApplicationUser: IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        [JsonIgnore]
        public virtual UserMetricEntity? UserMetric { get; set; }
    }
}