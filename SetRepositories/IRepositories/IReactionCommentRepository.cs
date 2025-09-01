using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blog.utils.Responses.ReactionComment;
using Blog.entities;
using Blog.entities.enums;
using Blog.utils;

namespace Blog.SetRepositories.IRepositories
{
    public interface IReactionCommentRepository
    {
        Task<ReactionCommentResponse> Reaction(CommentEntity comment, ApplicationUser user, LikeOrDislike newAction);
        Task<ReactionCommentEntity?> Remove(ulong Id);
        Task<bool> Exists(ApplicationUser user, CommentEntity comment);
        Task<PaginatedList<ReactionCommentEntity>> GetAllOfUserPaginated(ApplicationUser user, int pageNumber, int pageSize);
        Task<PaginatedList<ReactionCommentEntity>> GetAllOfCommentPaginated(CommentEntity comment, int pageNumber, int pageSize);
    }
}