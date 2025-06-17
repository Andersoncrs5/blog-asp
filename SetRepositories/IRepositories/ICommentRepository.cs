using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.DTOs.Comment;
using Blog.entities;
using Blog.utils;

namespace Blog.SetRepositories.IRepositories
{
    public interface ICommentRepository
    {
        Task<CommentEntity> Get(ulong Id, bool includeRelated = false);
        Task Delete(CommentEntity comment);
        Task<PaginatedList<CommentEntity>> GetAllOfUser(ApplicationUser user, int pageNumber, int pageSize);
        Task<PaginatedList<CommentEntity>> GetAllOfPost(PostEntity post, int pageNumber, int pageSize);
        Task<PaginatedList<CommentEntity>> GetAllCommentOnComment(CommentEntity comment, int pageNumber, int pageSize);
        Task<CommentEntity> Create(ApplicationUser user, PostEntity post, CreateCommentDTO dto);
        Task<CommentEntity> CreateOnComment(ApplicationUser user, PostEntity post, CreateCommentDTO dto, CommentEntity comment);
        Task<CommentEntity> Update(CommentEntity comment, UpdateCommentDTO dto);

    }
}