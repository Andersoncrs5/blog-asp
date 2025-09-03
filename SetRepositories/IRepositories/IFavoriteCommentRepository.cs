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
        Task<FavoriteCommentEntity?> Get(ulong Id);
        Task<bool> CheckExistsCommentWithFavorite(string userId, ulong commentId);
        IQueryable<FavoriteCommentEntity> GetAllOfUser(ApplicationUser user);
        IQueryable<FavoriteCommentEntity> GetAllOfComment(CommentEntity comment);
        Task<FavoriteCommentEntity> Save(ApplicationUser user, CommentEntity comment);
        Task Remove(FavoriteCommentEntity favorite);
        Task<bool> Exists(ApplicationUser user, CommentEntity comment);
    }
}