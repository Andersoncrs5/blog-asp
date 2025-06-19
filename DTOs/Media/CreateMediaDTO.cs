using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.entities.enums;

namespace Blog.DTOs.Media
{
    public class CreateMediaDTO
    {
        [Required]
        [StringLength(1000)]
        public string Url { get; set; } = string.Empty;
        public MediaTypeEnum MediaType { get; set; } = MediaTypeEnum.IMAGE;
        [StringLength(500)]
        public string? Description { get; set; }
        public int? Order { get; set; }

    }
}