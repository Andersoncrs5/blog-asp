using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.entities.enums;

namespace blog.DTOs.ReactionPost
{
    public class ReactionPostDTO
    {
        [Required] public long PostId { get; set; }
        [Required] public LikeOrDislike Action { get; set; }
    }
}