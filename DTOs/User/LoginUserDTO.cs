using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.DTOs.User
{
    public class LoginUserDTO
    {
        [Required(ErrorMessage = "Field is required")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        [StringLength(150, ErrorMessage = "Max size of 150")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Field is required")]
        [StringLength(50, ErrorMessage = "Max size of 50", MinimumLength = 6)] 
        public string Password { get; set; } = string.Empty;
    }
}