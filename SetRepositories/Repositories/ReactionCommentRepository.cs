using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blog.utils.enums;
using blog.utils.Responses.ReactionComment;
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

        public async Task<ReactionCommentResponse> Reaction(CommentEntity comment, ApplicationUser user, LikeOrDislike newAction)
        {
            ReactionCommentEntity? existingReaction = await _context.ReactionCommentEntities
                .FirstOrDefaultAsync(rc => rc.CommentId == comment.Id && rc.ApplicationUserId == user.Id);

            if (existingReaction != null && existingReaction.Reaction == newAction)
            {
                _context.ReactionCommentEntities.Remove(existingReaction);
                await _context.SaveChangesAsync();
                return new ReactionCommentResponse
                {
                    ReactionEntity = null,
                    ChangeType = ReactionCommentChangeType.Removed,
                    OldReaction = newAction,
                    NewReaction = null
                };
            }

            if (existingReaction != null && existingReaction.Reaction != newAction)
            {
                LikeOrDislike oldReactionType = existingReaction.Reaction;
                existingReaction.Reaction = newAction;
                existingReaction.UpdatedAt = DateTime.UtcNow; 

                await _context.SaveChangesAsync();
                return new ReactionCommentResponse
                {
                    ReactionEntity = existingReaction,
                    ChangeType = ReactionCommentChangeType.Updated,
                    OldReaction = oldReactionType, 
                    NewReaction = newAction
                };
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
                return new ReactionCommentResponse
                {
                    ReactionEntity = result.Entity,
                    ChangeType = ReactionCommentChangeType.Added,
                    OldReaction = null, 
                    NewReaction = newAction 
                };
            }

            throw new InvalidOperationException("Erro inesperado ao processar a reação no comentário.");
        }

        public async Task<ReactionCommentEntity?> Remove(ulong Id)
        {
            if (Id == 0)
                throw new ArgumentNullException(nameof(Id));

            ReactionCommentEntity? reaction = await _context.ReactionCommentEntities
                .FirstOrDefaultAsync(rc => rc.Id == Id);

            if (reaction == null)
                return null;

            _context.ReactionCommentEntities.Remove(reaction);
            await _context.SaveChangesAsync();
            return reaction;
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
        
        public async Task<PaginatedList<ReactionCommentEntity>> GetAllOfCommentPaginated(CommentEntity comment, int pageNumber, int pageSize)
        {
            IQueryable<ReactionCommentEntity> query = _context.ReactionCommentEntities
                .AsNoTracking()
                .Include(rp => rp.ApplicationUser) 
                .Where(rp => rp.CommentId == comment.Id);

            return await PaginatedList<ReactionCommentEntity>.CreateAsync(query, pageNumber, pageSize);
        }

    }
}