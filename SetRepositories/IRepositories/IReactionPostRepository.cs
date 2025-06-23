using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blog.utils.Responses.ReactionPost;
using Blog.entities;
using Blog.entities.enums;
using Blog.utils;

namespace Blog.SetRepositories.IRepositories
{
    public interface IReactionPostRepository
    {
        Task<bool> Exists(ApplicationUser user, PostEntity post);
        Task<ReactionPostResponse> ToggleReaction(ApplicationUser user, PostEntity post, LikeOrDislike newAction);
        Task Remove(ApplicationUser user, PostEntity post);
        Task<PaginatedList<ReactionPostEntity>> GetAllOfUserPaginated(ApplicationUser user, int pageNumber, int pageSize);
        Task<PaginatedList<ReactionPostEntity>> GetAllOfPostPaginated(PostEntity post, int pageNumber, int pageSize);
    }
}