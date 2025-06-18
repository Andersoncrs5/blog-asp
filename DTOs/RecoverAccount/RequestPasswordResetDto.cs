using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.DTOs.RecoverAccount
{
    public class RequestPasswordResetDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [StringLength(45)] 
        public string? RequestIpAddress { get; set; }

        [StringLength(512)]
        public string? RequestUserAgent { get; set; }
    }
}