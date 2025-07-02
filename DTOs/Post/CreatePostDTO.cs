using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.entities;

namespace Blog.DTOs.Post
{
    public class CreatePostDTO
    {
        [Required(ErrorMessage = "Field is required")]
        [StringLength(300, ErrorMessage = "Max size of 300", MinimumLength = 30)]
        public string Title { get; set; } = string.Empty; 

        [Required(ErrorMessage = "Field is required")]
        [StringLength(3000, ErrorMessage = "Max size of 3000", MinimumLength = 200)]
        public string Content { get; set; } = string.Empty;

        [Required] public long ReadTimes { get; set; } = 5;

        [Required]
        public long categoryId { get; set; }

        public PostEntity MappearToPostEntity() {
            return new PostEntity{
                Title = Title,
                Content = Content,
                ReadTimes = ReadTimes,
            };
        }
    }
}