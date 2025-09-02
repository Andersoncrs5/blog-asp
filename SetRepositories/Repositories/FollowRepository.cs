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
    public class FollowRepository : IFollowRepository
    {
        private readonly AppDbContext _context;

        public FollowRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Exists(string followerId, string followedId)
        {
            return await _context.FollowsEntities.AsNoTracking()
                .AnyAsync(f => f.FollowerId == followerId && f.FollowedId == followedId);
        }

        public async Task<FollowEntity> FollowAsync(ApplicationUser follower, ApplicationUser followed)
        {
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

        public async Task<FollowEntity?> GetAsync(string followerId, string followedId)
        {
            FollowEntity? follow = await _context.FollowsEntities
                .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowedId == followedId);

            if (follow is null)
            {
                return null;
            }

            return follow;
        }

        public async Task UnfollowAsync(FollowEntity follow)
        { 
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

        public async Task<FollowEntity> ChangeStatusReceiveNotifications(FollowEntity follow)
        {
            follow.ReceiveNotifications = !follow.ReceiveNotifications;

            _context.Entry(follow).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return follow;
        }

    }
}
