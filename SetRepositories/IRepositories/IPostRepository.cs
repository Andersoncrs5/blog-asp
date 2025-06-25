using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.DTOs.Post;
using Blog.entities;
using Blog.utils;

namespace Blog.SetRepositories.IRepositories
{
    public interface IPostRepository
    {
        Task<PostEntity> Get(long Id);
        Task<PostEntity> Delete(PostEntity post, ApplicationUser user);
        Task<PostEntity> Create(ApplicationUser user, PostEntity post, CategoryEntity category);
        Task<PaginatedList<PostEntity>> GetAllOfUserPaginated(ApplicationUser user, int pageNumber, int pageSize, bool IsActived = true);
        Task<PaginatedList<PostEntity>> GetAllPaginated(int pageNumber, int pageSize);
        Task<PostEntity> Update(PostEntity postExist, UpdatePostDTO dto, ApplicationUser user);
        Task<PostEntity> ChangeStatusActive(PostEntity post, ApplicationUser user);
        Task<PaginatedList<PostEntity>> GetAllToMePaginated( ApplicationUser currentUser,int pageNumber,int pageSize,bool includeRelations = true);
    }
}