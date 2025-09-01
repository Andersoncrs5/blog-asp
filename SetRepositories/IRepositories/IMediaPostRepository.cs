using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.DTOs.Media;
using Blog.entities;
using Blog.utils;

namespace Blog.SetRepositories.IRepositories
{
    public interface IMediaPostRepository
    {
        Task<MediaPostEntity?> GetAsync(ulong Id);
        Task<int> CheckAmountMediaByPost(long postId);
        Task DeleteAsync(MediaPostEntity media);
        Task<PaginatedList<MediaPostEntity>> GetAllOfPostPaginatedListAsync(PostEntity post, int pageNumber, int pageSize);
        Task<MediaPostEntity> CreateAsync(PostEntity post, CreateMediaDTO dto);
        Task<MediaPostEntity> UpdateAsync(MediaPostEntity media, UpdateMediaDTO dto);
    }
}