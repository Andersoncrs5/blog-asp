using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Context;
using Blog.entities;
using Blog.entities.enums;
using Blog.SetRepositories.IRepositories;
using Blog.utils;
using Microsoft.EntityFrameworkCore;

namespace Blog.SetRepositories.Repositories
{
    public class ReactionCommentRepository: IReactionCommentRepository
    {
        private readonly AppDbContext _context;

        public ReactionCommentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ReactionCommentEntity?> Reaction(CommentEntity comment, ApplicationUser user, LikeOrDislike newAction)
        {
            ReactionCommentEntity? existingReaction = await _context.ReactionCommentEntities 
                .FirstOrDefaultAsync(rc => rc.CommentId == comment.Id && rc.ApplicationUserId == user.Id);

            if (existingReaction != null && existingReaction.Reaction == newAction)
            {
                _context.ReactionCommentEntities.Remove(existingReaction); 
                await _context.SaveChangesAsync();
                return null; 
            }

            if (existingReaction != null && existingReaction.Reaction != newAction)
            {
                existingReaction.Reaction = newAction; 
                existingReaction.UpdatedAt = DateTime.UtcNow; 

                await _context.SaveChangesAsync();
                return existingReaction; 
            }

            if (existingReaction == null)
            {
                ReactionCommentEntity newReaction = new ReactionCommentEntity
                {
                    CommentId = comment.Id,
                    ApplicationUserId = user.Id,
                    Reaction = newAction,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _context.ReactionCommentEntities.AddAsync(newReaction);
                await _context.SaveChangesAsync();
                return result.Entity;
            }

            throw new InvalidOperationException("Error the reactioning on comment!");
        }

        public async Task Remove(ulong Id)
        {
            ReactionCommentEntity? reaction = await _context.ReactionCommentEntities 
                .FirstOrDefaultAsync(rc => rc.Id == Id);

            if (reaction == null)
                throw new ResponseException("Reaction not found", 404);
            
            _context.ReactionCommentEntities.Remove(reaction);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> Exists(ApplicationUser user, CommentEntity comment)
        {
            int exists = await _context.ReactionCommentEntities.AsNoTracking()
                .CountAsync(c => c.ApplicationUserId == user.Id && c.CommentId == comment.Id);

            return exists > 0;
        }

        public async Task<PaginatedList<ReactionCommentEntity>> GetAllOfUserPaginated(ApplicationUser user, int pageNumber, int pageSize)
        {
            IQueryable<ReactionCommentEntity> query = _context.ReactionCommentEntities
                .AsNoTracking()
                .Include(rp => rp.Comment) 
                .Where(rp => rp.ApplicationUserId == user.Id);

            return await PaginatedList<ReactionCommentEntity>.CreateAsync(query, pageNumber, pageSize);
        }
        
        public async Task<PaginatedList<ReactionCommentEntity>> GetAllOfPostPaginated(CommentEntity comment, int pageNumber, int pageSize)
        {
            IQueryable<ReactionCommentEntity> query = _context.ReactionCommentEntities
                .AsNoTracking()
                .Include(rp => rp.ApplicationUser) 
                .Where(rp => rp.CommentId == comment.Id);

            return await PaginatedList<ReactionCommentEntity>.CreateAsync(query, pageNumber, pageSize);
        }

    }
}