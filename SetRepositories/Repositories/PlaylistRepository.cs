using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Context;
using Blog.DTOs.Playlist;
using Blog.entities;
using Blog.SetRepositories.IRepositories;
using Blog.utils;
using Blog.utils.enums;
using Microsoft.EntityFrameworkCore;

namespace Blog.SetRepositories.Repositories
{
    public class PlaylistRepository: IPlaylistRepository
    {
        private readonly AppDbContext _context;

        public PlaylistRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PlaylistEntity> Create(ApplicationUser user, CreatePlaylistDTO dto)
        {
            int checkName = await _context.PlaylistEntities.AsNoTracking() 
                .CountAsync(p => p.ApplicationUserId == user.Id && p.Name == dto.Name);

            if (checkName > 0)
                throw new ResponseException("Playlist with this name already exists for this user", 400);

            PlaylistEntity play = new PlaylistEntity
            {
                Name = dto.Name,
                Description = dto.Description,
                IsPublic = false, 
                ApplicationUserId = user.Id,
                CreatedAt = DateTime.UtcNow 
            };

            var result = await _context.PlaylistEntities.AddAsync(play);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<PlaylistEntity?> Get(ulong Id)
        {
            if (Id == 0) 
                throw new ArgumentNullException(nameof(Id));

            PlaylistEntity? play = await _context.PlaylistEntities.AsNoTracking() 
                .FirstOrDefaultAsync(p => p.Id == Id);

            if (play is null)
                return null;

            return play;
        }

        public async Task<PlaylistEntity> Update(PlaylistEntity play, UpdatePlaylistDTO dto)
        {
            int checkNameConflict = await _context.PlaylistEntities.AsNoTracking()
                .CountAsync(p => p.ApplicationUserId == play.ApplicationUserId && 
                                 p.Name == dto.Name &&
                                 p.Id != play.Id); 

            if (checkNameConflict > 0)
                throw new ResponseException("Another playlist with this name already exists for this user", 400);

            play.Name = dto.Name;
            play.Description = dto.Description;
            play.IsPublic = false; 
            play.UpdatedAt = DateTime.UtcNow;

            _context.Entry(play).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return play;
        }

        public async Task Delete(PlaylistEntity play)
        {
            _context.PlaylistEntities.Remove(play);
            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedList<PlaylistEntity>> GetAllOfUserPaginated(
            ApplicationUser user, int pageNumber, int pageSize, bool showPublic = false)
        {
            IQueryable<PlaylistEntity> query = _context.PlaylistEntities.AsNoTracking() 
                .Include(p => p.ApplicationUser) 
                .Where(f => f.ApplicationUserId == user.Id && f.IsPublic == showPublic);

            return await PaginatedList<PlaylistEntity>.CreateAsync(query, pageNumber, pageSize);
        }
    
        public async Task<PlaylistEntity> ChangeStatusIsPublic(PlaylistEntity play)
        {
            play.IsPublic = !play.IsPublic;
            _context.Entry(play).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return play;
        }

        public async Task SumOrReduceItemCount(PlaylistEntity play, SumOrRedEnum action)
        {
            if (action == SumOrRedEnum.SUM) 
            {
                play.ItemCount += 1;
            }

            if (action == SumOrRedEnum.REDUCE) 
            {
                play.ItemCount -= 1;
            }

            _context.Entry(play).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public IQueryable<PlaylistEntity> GetAllOfUserQuery(ApplicationUser user)
        {
            return _context.PlaylistEntities.AsNoTracking().Where(p => p.ApplicationUserId == user.Id);
        }

    }
}