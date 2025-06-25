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

        public async Task<PostEntity> Get(long Id) 
        {
            if(long.IsNegative(Id)) 
                throw new ResponseException("Id is required");

            PostEntity? post = await _context.PostEntities.AsNoTracking()
                .Include(p => p.PostMetricEntity)
                .FirstOrDefaultAsync(u => u.Id == Id);

            if (post is null) 
                throw new ResponseException("Post not found", 404);

            return post;
        }
    
        public async Task<PostEntity> Delete(PostEntity post, ApplicationUser user) 
        {
            if (post.ApplicationUserId == user.Id)
                throw new ResponseException("You cannot to delete this post!!", 403);

            _context.PostEntities.Remove(post);
            await _context.SaveChangesAsync();
            return post;
        }
    
        public async Task<PostEntity> Create(ApplicationUser user, PostEntity post, CategoryEntity category)
        {
            post.ApplicationUserId = user.Id;
            post.categoryId = category.Id;
            
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

        public async Task<PaginatedList<PostEntity>> GetAllOfUserPaginated(ApplicationUser user, int pageNumber, int pageSize, bool IsActived = true)
        {
            IQueryable<PostEntity> query = _context.PostEntities
                .AsNoTracking()
                .Where(p => p.ApplicationUserId == user.Id && p.IsActived == IsActived);

            return await PaginatedList<PostEntity>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<PaginatedList<PostEntity>> GetAllToMePaginated( ApplicationUser currentUser,int pageNumber,int pageSize,bool includeRelations = true)
        {
            List<string> followedUserIds = await _context.FollowsEntities
                .AsNoTracking()
                .Where(f => f.FollowerId == currentUser.Id)
                .Select(f => f.FollowedId)
                .ToListAsync();

            List<long> preferredCategoryIds = await _context.UserPreferenceEntities
                .AsNoTracking()
                .Where(up => up.ApplicationUserId == currentUser.Id)
                .Select(up => up.CategoryId)
                .ToListAsync();

            IQueryable<PostEntity> activePostsBaseQuery = _context.PostEntities
                .AsNoTracking()
                .Where(p => p.IsActived == true);

            IQueryable<PostEntity> followedPostsQuery = activePostsBaseQuery
                .Where(p => followedUserIds.Contains(p.ApplicationUserId));

            IQueryable<PostEntity> preferredPostsQuery = activePostsBaseQuery
                .Where(p => preferredCategoryIds.Contains(p.categoryId));

            IQueryable<PostEntity> generalPostsQuery = activePostsBaseQuery;

            IQueryable<PostEntity> combinedQuery = followedPostsQuery
                                                    .Union(preferredPostsQuery)
                                                    .Union(generalPostsQuery);

            combinedQuery = combinedQuery.OrderByDescending(p => p.CreatedAt);

            if (includeRelations)
            {
                combinedQuery = combinedQuery
                    .Include(p => p.ApplicationUser)
                    .Include(p => p.MediaPostEntities)           
                    .Include(p => p.Category);       
            }

            return await PaginatedList<PostEntity>.CreateAsync(combinedQuery, pageNumber, pageSize);
        }

        public async Task<PaginatedList<PostEntity>> GetAllPaginated(int pageNumber, int pageSize)
        {
            IQueryable<PostEntity> query = _context.PostEntities
                .AsNoTracking()
                .Where(p => p.IsActived == true);

            return await PaginatedList<PostEntity>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<PostEntity> Update(PostEntity postExist, UpdatePostDTO dto, ApplicationUser user) 
        {
            if (postExist.ApplicationUserId == user.Id)
                throw new ResponseException("You cannot to update this post!!", 403);

            postExist.Title = dto.Title;
            postExist.Content = dto.Content;

            _context.Entry(postExist).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return postExist;
        }

    }
}