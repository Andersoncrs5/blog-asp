using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.entities;
using Blog.entities.enums;
using Blog.utils;

namespace Blog.SetRepositories.IRepositories
{
    public interface IReactionCommentRepository
    {
        Task<ReactionCommentEntity?> Reaction(CommentEntity comment, ApplicationUser user, LikeOrDislike newAction);
        Task Remove(CommentEntity comment, ApplicationUser user);
        Task<bool> Exists(ApplicationUser user, CommentEntity comment);
        Task<PaginatedList<ReactionCommentEntity>> GetAllOfUserPaginated(ApplicationUser user, int pageNumber, int pageSize);
        Task<PaginatedList<ReactionCommentEntity>> GetAllOfCommentPaginated(CommentEntity comment, int pageNumber, int pageSize);
    }
}