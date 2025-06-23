using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blog.utils.enums;
using Blog.entities;
using Blog.entities.enums;

namespace blog.utils.Responses.ReactionPost
{
    public class ReactionPostResponse
    {
        public ReactionPostEntity? ReactionEntity { get; set; }

        public ReactionPostChangeType ChangeType { get; set; }

        public LikeOrDislike? OldReaction { get; set; }

        public LikeOrDislike? NewReaction { get; set; }
    }
}