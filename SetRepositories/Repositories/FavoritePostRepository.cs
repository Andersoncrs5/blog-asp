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
    public class FavoritePostRepository : IFavoritePostRepository
    {
        private readonly AppDbContext _context;

        public FavoritePostRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<FavoritePostEntity>> GetAllOfUserPaginated(ApplicationUser user, int pageNumber, int pageSize)
        {
            IQueryable<FavoritePostEntity> query = _context.FavoritePostEntities
                .Include(f => f.ApplicationUser)
                .Include(f => f.Post)
                .AsNoTracking().Where(f => f.ApplicationUserId == user.Id);

            return await PaginatedList<FavoritePostEntity>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<PaginatedList<FavoritePostEntity>> GetAllOfPostPaginated(PostEntity post, int pageNumber, int pageSize)
        {
            IQueryable<FavoritePostEntity> query = _context.FavoritePostEntities
                .Include(f => f.ApplicationUser)
                .Include(f => f.Post)
                .AsNoTracking().Where(f => f.PostId == post.Id);

            return await PaginatedList<FavoritePostEntity>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<bool> CheckExistsPostWithFavorite(string userId, long postId)
        {
            return await _context.FavoritePostEntities
                .AsNoTracking()
                .AnyAsync(f => f.ApplicationUserId == userId && f.PostId == postId);
        }

        public async Task<FavoritePostEntity> Save(ApplicationUser user, PostEntity post) 
        {   
            FavoritePostEntity save = new FavoritePostEntity()
            {
                ApplicationUserId = user.Id,
                PostId = post.Id
            };

            var result = await _context.FavoritePostEntities.AddAsync(save);
            await _context.SaveChangesAsync();

            return result.Entity;
        }

        public async Task Remove(FavoritePostEntity favorite) 
        {
            _context.Remove(favorite);
            await _context.SaveChangesAsync();
        }

        public async Task<FavoritePostEntity?> Get(long Id)
        {
            FavoritePostEntity? save = await _context.FavoritePostEntities
                .AsNoTracking().FirstOrDefaultAsync(f => f.Id == Id);

            if (save is null)
                return null;

            return save;
        }

        public async Task<bool> Exists(ApplicationUser user, PostEntity post)
        {
            int check = await _context.FavoritePostEntities
                .AsNoTracking().CountAsync(f => f.ApplicationUserId == user.Id && f.PostId == post.Id);

            return check > 0;
        }

    }
}