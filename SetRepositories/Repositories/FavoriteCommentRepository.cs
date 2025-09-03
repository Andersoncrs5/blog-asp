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
    public class FavoriteCommentRepository : IFavoriteCommentRepository
    {
        private readonly AppDbContext _context;

        public FavoriteCommentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<FavoriteCommentEntity?> Get(ulong Id)
        {
            FavoriteCommentEntity? favorite = await _context.FavoriteCommentEntities.AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == Id);

            if (favorite is null)
                return null;

            return favorite;
        }

        public IQueryable<FavoriteCommentEntity> GetAllOfUser(ApplicationUser user)
        {
            IQueryable<FavoriteCommentEntity> query = _context.FavoriteCommentEntities
                .Include(f => f.ApplicationUser)
                .Include(f => f.Comment)
                .AsNoTracking().Where(f => f.ApplicationUserId == user.Id);

            return query;
        }

        public IQueryable<FavoriteCommentEntity> GetAllOfComment(CommentEntity comment)
        {
            IQueryable<FavoriteCommentEntity> query = _context.FavoriteCommentEntities
                .Include(f => f.ApplicationUser)
                .Include(f => f.Comment)
                .AsNoTracking().Where(f => f.CommentId == comment.Id);

            return query;
        }

        public async Task<bool> CheckExistsCommentWithFavorite(string userId, ulong commentId)
        {
            return await _context.FavoriteCommentEntities
                .AsNoTracking()
                .AnyAsync(f => f.ApplicationUserId.Contains(userId) && f.CommentId == commentId);
        }

        public async Task<FavoriteCommentEntity> Save(ApplicationUser user, CommentEntity comment)
        {
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