using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.DTOs.Media;
using Blog.entities;

namespace Blog.SetRepositories.IRepositories
{
    public interface IMediaPostRepository
    {
        Task<MediaPostEntity> GetAsync(ulong Id);
        Task DeleteAsync(MediaPostEntity media);
        Task<List<MediaPostEntity>> GetAllOfPost(PostEntity post);
        Task<MediaPostEntity> CreateAsync(PostEntity post, CreateMediaDTO dto);
        Task<MediaPostEntity> UpdateAsync(MediaPostEntity media, UpdateMediaDTO dto);
    }
}