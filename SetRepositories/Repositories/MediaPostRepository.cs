using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Context;
using Blog.DTOs.Media;
using Blog.entities;
using Blog.SetRepositories.IRepositories;
using Blog.utils;
using Microsoft.EntityFrameworkCore;

namespace Blog.SetRepositories.Repositories
{
    public class MediaPostRepository: IMediaPostRepository
    {
        private readonly AppDbContext _context;

        public MediaPostRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MediaPostEntity> GetAsync(ulong Id)
        {
            if (Id == 0)
                throw new ResponseException("Media ID is required and must be positive.", 400);

            MediaPostEntity? media = await _context.MediaPostEntities.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (media is null)
                throw new ResponseException("Media not found");

            return media;
        }

        public async Task DeleteAsync(MediaPostEntity media)
        {
            _context.MediaPostEntities.Remove(media);
            await _context.SaveChangesAsync();
        }

        public async Task<List<MediaPostEntity>> GetAllOfPost(PostEntity post)
        {
            return await _context.MediaPostEntities.AsNoTracking()
                .Where(e => e.PostId == post.Id)
                .OrderBy(e => e.Order ?? int.MaxValue)
                .ThenBy(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<MediaPostEntity> CreateAsync(PostEntity post, CreateMediaDTO dto)
        {
            int count = await _context.MediaPostEntities.AsNoTracking()
                .CountAsync(e => e.PostId == post.Id);

            if (count >= 10)
                throw new ResponseException("Limit max to media numbers!");

            MediaPostEntity media = new MediaPostEntity()
            {
                Description = dto.Description,
                MediaType = dto.MediaType,
                Url = dto.Url,
                Order = dto.Order,
                PostId = post.Id,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _context.MediaPostEntities.AddAsync(media);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<MediaPostEntity> UpdateAsync(MediaPostEntity media, UpdateMediaDTO dto)
        {
            media.Url = dto.Url;
            media.Description = dto.Description;
            media.Order = dto.Order;
            media.MediaType = dto.MediaType;

            _context.Entry(media).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return media;
        }

    }
}