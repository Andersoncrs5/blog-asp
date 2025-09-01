using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Context;
using Blog.DTOs.Post;
using Blog.entities;
using Blog.SetRepositories.IRepositories;
using Blog.utils;
using Microsoft.EntityFrameworkCore;

namespace Blog.SetRepositories.Repositories
{
    public class PostRepository: IPostRepository
    {
        private readonly AppDbContext _context;

        public PostRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PostEntity?> Get(long Id) 
        {
            if(long.IsNegative(Id) || Id == 0)
                throw new ArgumentNullException(nameof(Id));

            PostEntity? post = await _context.PostEntities.AsNoTracking()
                .Include(p => p.PostMetricEntity)
                .FirstOrDefaultAsync(u => u.Id == Id);

            if (post is null)
                return null;

            return post;
        }
    
        public async Task<PostEntity> Delete(PostEntity post, ApplicationUser user) 
        {
            if (post.ApplicationUserId != user.Id)
                throw new ResponseException("You cannot to delete this post!!", 403);

            _context.PostEntities.Remove(post);
            await _context.SaveChangesAsync();
            return post;
        }
    
        public async Task<PostEntity> Create(ApplicationUser user, PostEntity post, CategoryEntity category)
        {
            post.ApplicationUserId = user.Id;
            post.CategoryId = category.Id;

            post.IsActived = true;
            var result = await _context.PostEntities.AddAsync(post);
            await _context.SaveChangesAsync();

            return result.Entity;
        }
        
        public async Task<PostEntity> ChangeStatusActive(PostEntity post, ApplicationUser user) 
        {
            post.IsActived = !post.IsActived;
            _context.Entry(post).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return post;
        }

        public IQueryable<PostEntity> GetAllOfUser(ApplicationUser user, bool IsActived = true)
        {
            IQueryable<PostEntity> query = _context.PostEntities
                .AsNoTracking()
                .Where(p => p.ApplicationUserId.Contains(user.Id) && p.IsActived == IsActived);

            return query;
        }

        public async Task<IQueryable<PostEntity>> GetAllToMe(ApplicationUser currentUser, bool includeRelations = true)
        {
            List<string> followedUserIds = await _context.FollowsEntities
                .AsNoTracking()
                .Where(f => f.FollowerId.Contains(currentUser.Id))
                .Select(f => f.FollowedId)
                .ToListAsync();

            List<long> preferredCategoryIds = await _context.UserPreferenceEntities
                .AsNoTracking()
                .Where(up => up.ApplicationUserId.Contains(currentUser.Id))
                .Select(up => up.CategoryId)
                .ToListAsync();

            IQueryable<PostEntity> activePostsBaseQuery = _context.PostEntities
                .AsNoTracking()
                .Where(p => p.IsActived == true);

            IQueryable<PostEntity> followedPostsQuery = activePostsBaseQuery
                .Where(p => followedUserIds.Contains(p.ApplicationUserId));

            IQueryable<PostEntity> preferredPostsQuery = activePostsBaseQuery
                .Where(p => preferredCategoryIds.Contains(p.CategoryId));

            IQueryable<PostEntity> generalPostsQuery = activePostsBaseQuery;

            IQueryable<PostEntity> combinedQuery = followedPostsQuery
                                                    .Union(preferredPostsQuery)
                                                    .Union(generalPostsQuery);

            if (includeRelations)
            {
                combinedQuery = combinedQuery
                    .Include(p => p.ApplicationUser)
                    .Include(p => p.MediaPostEntities)           
                    .Include(p => p.Category);       
            }

            return combinedQuery;
        }

        public IQueryable<PostEntity> GetAll()
        {
            IQueryable<PostEntity> query = _context.PostEntities
                .AsNoTracking()
                .Where(p => p.IsActived == true);

            return query;
        }

        public async Task<PostEntity> Update(PostEntity postExist, UpdatePostDTO dto, ApplicationUser user) 
        {
            if (postExist.ApplicationUserId != user.Id)
                throw new ResponseException("You cannot to update this post!!", 403);

            postExist.Title = dto.Title;
            postExist.Content = dto.Content;

            _context.Entry(postExist).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return postExist;
        }

        public async Task CalculateEngagementScore(PostEntity post, PostMetricEntity metric)
        {
            double scorePositive = (metric.Likes + metric.FavoriteCount + metric.Viewed + metric.MediaCount + metric.CommentCount)/5;
            double scoreNegavite = (metric.DisLikes + metric.ReportsReceivedCount)/2;
            post.EngagementScore = scorePositive - scoreNegavite;
            _context.Entry(post).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

    }
}