using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blog.entities;
using Blog.entities;
using Blog.utils;

namespace blog.SetRepositories.IRepositories
{
    public interface IFollowRepository
    {
        Task<FollowEntity> FollowAsync(ApplicationUser follower, ApplicationUser followed);
        Task UnfollowAsync(ApplicationUser follower, ApplicationUser followed);
        Task<PaginatedList<FollowEntity>> GetFollowingAsync(ApplicationUser follower, int pageNumber, int pageSize, bool includeRelations = true);
        Task<PaginatedList<FollowEntity>> GetFollowersAsync(ApplicationUser followed, int pageNumber, int pageSize, bool includeRelations = true);
    }
}