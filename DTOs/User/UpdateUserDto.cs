using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.DTOs.User
{
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "Field is required")]
        [StringLength(100, ErrorMessage = "Max size of 100")]
        public string Name { get; set; } = string.Empty; 

        [Required(ErrorMessage = "Field is required")]
        [StringLength(50, ErrorMessage = "Max size of 50", MinimumLength = 6)] 
        public string Password { get; set; } = string.Empty;
    }
    
}