using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blog.utils.enums;
using blog.utils.Responses.ReactionPost;
using Blog.Context;
using Blog.entities;
using Blog.entities.enums;
using Blog.SetRepositories.IRepositories;
using Blog.utils;
using Microsoft.EntityFrameworkCore;

namespace Blog.SetRepositories.Repositories
{
    public class ReactionPostRepository: IReactionPostRepository
    {
        private readonly AppDbContext _context;

        public ReactionPostRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<ReactionPostResponse> ToggleReaction(ApplicationUser user, PostEntity post, LikeOrDislike newAction)
        {
            ReactionPostEntity? existingReaction = await _context.ReactionPostEntities 
                .FirstOrDefaultAsync(rp => rp.ApplicationUserId == user.Id && rp.PostId == post.Id);

            if (existingReaction != null && existingReaction.Reaction == newAction)
            {
                _context.ReactionPostEntities.Remove(existingReaction);
                await _context.SaveChangesAsync();
                return new ReactionPostResponse
                {
                    ReactionEntity = null, 
                    ChangeType = ReactionPostChangeType.Removed,
                    OldReaction = newAction, 
                    NewReaction = null
                };
            }

            if (existingReaction != null && existingReaction.Reaction != newAction)
            {
                LikeOrDislike oldReactionType = existingReaction.Reaction; 
                existingReaction.Reaction = newAction;
                existingReaction.CreatedAt = DateTime.UtcNow; 
                
                await _context.SaveChangesAsync();
                return new ReactionPostResponse
                {
                    ReactionEntity = existingReaction,
                    ChangeType = ReactionPostChangeType.Updated,
                    OldReaction = oldReactionType, 
                    NewReaction = newAction 
                };
            }

            if (existingReaction == null)
            {
                ReactionPostEntity newReaction = new ReactionPostEntity
                {
                    ApplicationUserId = user.Id,
                    PostId = post.Id,
                    Reaction = newAction,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _context.ReactionPostEntities.AddAsync(newReaction);
                await _context.SaveChangesAsync();
                return new ReactionPostResponse
                {
                    ReactionEntity = result.Entity,
                    ChangeType = ReactionPostChangeType.Added,
                    OldReaction = null, 
                    NewReaction = newAction 
                };
            }
            throw new InvalidOperationException("Erro inesperado ao processar a reação no post.");
        }

        public async Task<bool> Exists(ApplicationUser user, PostEntity post)
        {
            int exists = await _context.ReactionPostEntities.AsNoTracking()
                .CountAsync(c => c.ApplicationUserId == user.Id && c.PostId == post.Id);

            return exists > 0;
        }

        public async Task<ReactionPostEntity> Remove(ApplicationUser user, PostEntity post)
        {
            ReactionPostEntity? reactionToRemove = await _context.ReactionPostEntities
                .FirstOrDefaultAsync(rp => rp.ApplicationUserId == user.Id && rp.PostId == post.Id);

            if (reactionToRemove is null)
                throw new ResponseException("Reaction not found", 404);

            _context.ReactionPostEntities.Remove(reactionToRemove);
            await _context.SaveChangesAsync();
            return reactionToRemove;
        }

        public async Task<PaginatedList<ReactionPostEntity>> GetAllOfUserPaginated(ApplicationUser user, int pageNumber, int pageSize)
        {
            IQueryable<ReactionPostEntity> query = _context.ReactionPostEntities
                .AsNoTracking()
                .Include(rp => rp.Post) 
                .Where(rp => rp.ApplicationUserId == user.Id);

            return await PaginatedList<ReactionPostEntity>.CreateAsync(query, pageNumber, pageSize);
        }
        
        public async Task<PaginatedList<ReactionPostEntity>> GetAllOfPostPaginated(PostEntity post, int pageNumber, int pageSize)
        {
            IQueryable<ReactionPostEntity> query = _context.ReactionPostEntities
                .AsNoTracking()
                .Include(rp => rp.ApplicationUser) 
                .Where(rp => rp.PostId == post.Id);

            return await PaginatedList<ReactionPostEntity>.CreateAsync(query, pageNumber, pageSize);
        }
    }
}