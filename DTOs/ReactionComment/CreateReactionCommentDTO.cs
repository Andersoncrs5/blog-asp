using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.entities.enums;

namespace blog.DTOs.ReactionComment
{
    public class CreateReactionCommentDTO
    {
        [Required] public ulong CommentId { get; set; }
        [Required] public LikeOrDislike action { get; set; }
    }
}