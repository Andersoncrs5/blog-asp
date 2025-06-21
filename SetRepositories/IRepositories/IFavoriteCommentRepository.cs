using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.entities;
using Blog.utils;

namespace Blog.SetRepositories.IRepositories
{
    public interface IFavoriteCommentRepository
    {
        Task<FavoriteCommentEntity> Get(ulong Id);
        Task<PaginatedList<FavoriteCommentEntity>> GetAllOfUserPaginated(ApplicationUser user, int pageNumber, int pageSize);
        Task<PaginatedList<FavoriteCommentEntity>> GetAllOfCommentPaginated(CommentEntity comment, int pageNumber, int pageSize);
        Task<FavoriteCommentEntity> SaveOrRemove(ApplicationUser user, CommentEntity comment);
        Task Remove(FavoriteCommentEntity favorite);
        Task<bool> Exists(ApplicationUser user, CommentEntity comment);
    }
}