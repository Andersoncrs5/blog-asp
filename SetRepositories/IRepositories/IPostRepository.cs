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
        IQueryable<PostEntity> GetAllOfUser(ApplicationUser user, bool IsActived = true);
        IQueryable<PostEntity> GetAll();
        Task<PostEntity> Update(PostEntity postExist, UpdatePostDTO dto, ApplicationUser user);
        Task<PostEntity> ChangeStatusActive(PostEntity post, ApplicationUser user);
        Task<IQueryable<PostEntity>> GetAllToMe(ApplicationUser currentUser, bool includeRelations = true);
        Task CalculateEngagementScore(PostEntity post, PostMetricEntity metric);
    }
}