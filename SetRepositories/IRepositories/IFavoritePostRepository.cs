using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.entities;
using Blog.utils;

namespace Blog.SetRepositories.IRepositories
{
    public interface IFavoritePostRepository
    {
        Task<PaginatedList<FavoritePostEntity>> GetAllOfUserPaginated(ApplicationUser user, int pageNumber, int pageSize);
        Task<bool> CheckExistsPostWithFavorite(string userId, long postId);
        Task<PaginatedList<FavoritePostEntity>> GetAllOfPostPaginated(PostEntity post, int pageNumber, int pageSize);
        Task<FavoritePostEntity> Save(ApplicationUser user, PostEntity post);
        Task Remove(FavoritePostEntity favorite);
        Task<FavoritePostEntity?> Get(long Id);
        Task<bool> Exists(ApplicationUser user, PostEntity post);

    }
}