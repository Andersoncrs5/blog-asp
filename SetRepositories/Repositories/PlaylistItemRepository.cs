using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Context;
using Blog.entities;
using Blog.SetRepositories.IRepositories;
using Blog.utils;
using Microsoft.EntityFrameworkCore;

namespace Blog.SetRepositories.Repositories
{
    public class PlaylistItemRepository: IPlaylistItemRepository
    {
        private readonly AppDbContext _context;

        public PlaylistItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Exists(ulong playlistId, long postId) 
        {
            return await _context.PlaylistItemEntities.AsNoTracking()
                .AnyAsync(pi => pi.PlaylistId == playlistId && pi.PostId == postId);
        }

        public async Task<PlaylistItemEntity> AddPostToPlaylist(PlaylistEntity playlist, PostEntity post, int? order = null)
        {
            PlaylistItemEntity newPlaylistItem = new PlaylistItemEntity
            {
                PlaylistId = playlist.Id,
                PostId = post.Id,
                Order = order,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _context.PlaylistItemEntities.AddAsync(newPlaylistItem);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task RemovePostFromPlaylist(PlaylistItemEntity playlistItem)
        {
            _context.PlaylistItemEntities.Remove(playlistItem);
            await _context.SaveChangesAsync();
        }

        public async Task<PlaylistItemEntity?> Get(ulong Id)
        {
            if (Id == 0)
                throw new ArgumentNullException(nameof(Id));

            PlaylistItemEntity? playlistItem = await _context.PlaylistItemEntities.AsNoTracking()
                .Include(pi => pi.Playlist)
                .Include(pi => pi.Post)
                .FirstOrDefaultAsync(pi => pi.Id == Id);

            if (playlistItem is null)
                return null;

            return playlistItem;
        }

        public async Task<PaginatedList<PlaylistItemEntity>> GetAllOfPlaylistPaginated(PlaylistEntity playlist, int pageNumber, int pageSize)
        {
            IQueryable<PlaylistItemEntity> query = _context.PlaylistItemEntities.AsNoTracking()
                .Include(pi => pi.Post) 
                .Where(pi => pi.PlaylistId == playlist.Id)
                .OrderBy(pi => pi.Order ?? long.MaxValue)
                .ThenBy(pi => pi.CreatedAt);

            return await PaginatedList<PlaylistItemEntity>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<PlaylistItemEntity> UpdateOrder(PlaylistItemEntity playlistItem, int newOrder)
        {
            playlistItem.Order = newOrder;
            await _context.SaveChangesAsync();
            return playlistItem;
        }
    }
}
