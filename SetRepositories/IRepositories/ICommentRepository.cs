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
        Task<CommentEntity> Get(ulong Id, bool includeRelated = false, bool includeMetric = false);
        Task Delete(CommentEntity comment);
        Task<PaginatedList<CommentEntity>> GetAllOfUserPaginatedList(ApplicationUser user, int pageNumber, int pageSize, bool includeRelated = false, bool includeMetric = false);
        Task<PaginatedList<CommentEntity>> GetAllOfPostPaginatedList(PostEntity post, int pageNumber, int pageSize);
        Task<PaginatedList<CommentEntity>> GetAllCommentOnCommentPaginatedList(CommentEntity comment, int pageNumber, int pageSize, bool includeRelated = false, bool includeMetric = false);
        Task<CommentEntity> Create(ApplicationUser user, PostEntity post, CreateCommentDTO dto, ulong? parentId);
        Task<CommentEntity> Update(CommentEntity comment, UpdateCommentDTO dto);

    }
}