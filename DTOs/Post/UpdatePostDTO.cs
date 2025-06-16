using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.entities;

namespace Blog.DTOs.Post
{
    public class UpdatePostDTO
    {
        [Required(ErrorMessage = "Field is required")]
        [StringLength(300, ErrorMessage = "Max size of 300")]
        public string Title { get; set; } = string.Empty; 

        [Required(ErrorMessage = "Field is required")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        [StringLength(3000, ErrorMessage = "Max size of 3000")]
        public string Content { get; set; } = string.Empty;

        [MaxLength(120)]
        [Required] public long ReadTimes { get; set; }

        public PostEntity MappearToPostEntity() {
            return new PostEntity{
                Title = Title,
                Content = Content,
                ReadTimes = ReadTimes,
            };
        }

    }
}