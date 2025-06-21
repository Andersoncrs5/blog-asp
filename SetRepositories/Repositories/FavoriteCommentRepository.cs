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
    public class FavoriteCommentRepository: IFavoriteCommentRepository
    {
        private readonly AppDbContext _context;

        public FavoriteCommentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<FavoriteCommentEntity> Get(ulong Id)
        {
            FavoriteCommentEntity? favorite = await _context.FavoriteCommentEntities.AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == Id);

            if (favorite is null)
                throw new ResponseException("Favorite not found");

            return favorite;
        }

        public async Task<PaginatedList<FavoriteCommentEntity>> GetAllOfUserPaginated(ApplicationUser user, int pageNumber, int pageSize)
        {
            IQueryable<FavoriteCommentEntity> query = _context.FavoriteCommentEntities
                .Include(f => f.ApplicationUser)
                .Include(f => f.Comment)
                .AsNoTracking().Where(f => f.ApplicationUserId == user.Id);

            return await PaginatedList<FavoriteCommentEntity>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<PaginatedList<FavoriteCommentEntity>> GetAllOfCommentPaginated(CommentEntity comment, int pageNumber, int pageSize)
        {
            IQueryable<FavoriteCommentEntity> query = _context.FavoriteCommentEntities
                .Include(f => f.ApplicationUser)
                .Include(f => f.Comment)
                .AsNoTracking().Where(f => f.CommentId == comment.Id);

            return await PaginatedList<FavoriteCommentEntity>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<FavoriteCommentEntity> SaveOrRemove(ApplicationUser user, CommentEntity comment)
        {
            FavoriteCommentEntity? check = await _context.FavoriteCommentEntities.AsNoTracking()
                .FirstOrDefaultAsync(f => f.ApplicationUserId == user.Id && f.CommentId == comment.Id);

            if (check is not null)
            {
                _context.FavoriteCommentEntities.Remove(check);
                await _context.SaveChangesAsync();
                return check;
            }

            FavoriteCommentEntity save = new FavoriteCommentEntity
            {
                ApplicationUserId = user.Id,
                CommentId = comment.Id,
                CreatedAt = DateTime.UtcNow
            };
            
            var result = await _context.FavoriteCommentEntities.AddAsync(save);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task Remove(FavoriteCommentEntity favorite)
        {
            _context.FavoriteCommentEntities.Remove(favorite);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> Exists(ApplicationUser user, CommentEntity comment)
        {
            int check = await _context.FavoriteCommentEntities
                .AsNoTracking().CountAsync(f => f.ApplicationUserId == user.Id && f.CommentId == comment.Id);

            return check > 0;
        }
    }
}