using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.entities;

namespace Blog.DTOs.Category
{
    public class CreateCategoryDTO
    {
        [Required]
        [StringLength(maximumLength:150, MinimumLength = 4)]
        public string Name { get; set; } = string.Empty;

        

        public CategoryEntity toCategoryEntity() 
        {
            return new CategoryEntity{
                Name = Name
            };
        }
    }
}