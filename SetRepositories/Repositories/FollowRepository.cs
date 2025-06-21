using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blog.entities;
using blog.SetRepositories.IRepositories;
using Blog.Context;
using Blog.entities;
using Blog.utils;
using Microsoft.EntityFrameworkCore;

namespace blog.SetRepositories.Repositories
{
    public class FollowRepository: IFollowRepository
    {
        private readonly AppDbContext _context;

        public FollowRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<FollowEntity> FollowAsync(ApplicationUser follower, ApplicationUser followed)
        {
            if (follower == null || followed == null || follower.Id == followed.Id)
            {
                throw new ResponseException("Invalid follower or followed user.", 400);
            }
            
            int check = await _context.FollowsEntities.AsNoTracking()
                .CountAsync(f => f.FollowerId == follower.Id && f.FollowedId == followed.Id);

            if (check > 0) {
                throw new ResponseException($"You are already following user: {followed.UserName ?? followed.Id}.", 400); 
            }

            FollowEntity follow = new FollowEntity
            {
                FollowerId = follower.Id,
                FollowedId = followed.Id,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _context.FollowsEntities.AddAsync(follow); 
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task UnfollowAsync(ApplicationUser follower, ApplicationUser followed) 
        {
            if (follower == null || followed == null)
            {
                throw new ResponseException("Invalid follower or followed user.", 400);
            }
            
            FollowEntity? follow = await _context.FollowsEntities
                .FirstOrDefaultAsync(f => f.FollowerId == follower.Id && f.FollowedId == followed.Id);

            if (follow is null) {
                throw new ResponseException($"You are not following user: {followed.UserName ?? followed.Id}.", 404); 
            }

            _context.FollowsEntities.Remove(follow); 
            await _context.SaveChangesAsync();
        }
        
        public async Task<PaginatedList<FollowEntity>> GetFollowingAsync(ApplicationUser follower, int pageNumber, int pageSize, bool includeRelations = true) 
        {
            if (follower == null)
            {
                throw new ResponseException("Follower user is required.", 400);
            }

            IQueryable<FollowEntity> query = _context.FollowsEntities 
                .AsNoTracking()
                .Where(f => f.FollowerId == follower.Id)
                .OrderBy(f => f.CreatedAt); 

            if (includeRelations)
            {
                query = query.Include(f => f.Followed);   
            }

            return await PaginatedList<FollowEntity>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<PaginatedList<FollowEntity>> GetFollowersAsync(ApplicationUser followed, int pageNumber, int pageSize, bool includeRelations = true) 
        {
            if (followed == null)
            {
                throw new ResponseException("Followed user is required.", 400);
            }

            IQueryable<FollowEntity> query = _context.FollowsEntities 
                .AsNoTracking()
                .Where(f => f.FollowedId == followed.Id)
                .OrderBy(f => f.CreatedAt); 

            if (includeRelations)
            {
                query = query.Include(f => f.Follower);
            }

            return await PaginatedList<FollowEntity>.CreateAsync(query, pageNumber, pageSize);
        }
        
    }
}
