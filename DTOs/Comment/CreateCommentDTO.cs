using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.DTOs.Comment
{
    public class CreateCommentDTO
    {
        [Required]
        [StringLength(maximumLength:350, MinimumLength = 1)]
        public string Content { get; set; } = string.Empty;
    }
}