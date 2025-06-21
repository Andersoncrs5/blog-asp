using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Context;
using Blog.DTOs.Comment;
using Blog.entities;
using Blog.SetRepositories.IRepositories;
using Blog.utils;
using Microsoft.EntityFrameworkCore;

namespace Blog.SetRepositories.Repositories
{
    public class CommentRepository: ICommentRepository
    {
        private readonly AppDbContext _context;

        public CommentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CommentEntity> Get(ulong Id, bool includeRelated = false, bool includeMetric = false)
        {

            IQueryable<CommentEntity> query = _context.CommentEntities.AsNoTracking();

            if (includeRelated)
            {
                query = query
                    .Include(c => c.ApplicationUser)
                    .Include(c => c.Post);          
            }

            if (includeMetric)
            {
                query = query
                    .Include(c => c.CommentMetric);
            }
        
            CommentEntity? comment = await query.FirstOrDefaultAsync(c => c.Id == Id);

            if (comment is null)
                throw new ResponseException("Comment not found", 404);

            return comment;
        }

        public async Task Delete(CommentEntity comment)
        {
            List<CommentEntity> comments = await _context.CommentEntities.AsNoTracking()
                .Where(c => c.ParentId == comment.Id).ToListAsync();

            _context.CommentEntities.RemoveRange(comments);

            _context.Remove(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedList<CommentEntity>> GetAllOfUserPaginatedList(ApplicationUser user, int pageNumber, int pageSize, bool includeRelated = false, bool includeMetric = false)
        {
            IQueryable<CommentEntity> query = _context.CommentEntities
                .AsNoTracking()
                .Where(c => c.ApplicationUserId == user.Id);

            if (includeRelated) 
            {
                query = query.Include(c => c.ApplicationUser).Include(c => c.Post);
            }
            
            if (includeMetric)
            {
                query = query
                    .Include(c => c.CommentMetric);
            }

            return await PaginatedList<CommentEntity>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<PaginatedList<CommentEntity>> GetAllOfPostPaginatedList(PostEntity post, int pageNumber, int pageSize)
        {
            IQueryable<CommentEntity> query = _context.CommentEntities.AsNoTracking()
                .Include(c => c.ApplicationUser)
                .Where(c => c.PostId == post.Id && c.ParentId == null);

            return await PaginatedList<CommentEntity>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<CommentEntity> Create(ApplicationUser user, PostEntity post, CreateCommentDTO dto, ulong? parentId = null) 
        {
            CommentEntity comment = new CommentEntity
            {
                Content = dto.Content,
                ApplicationUserId = user.Id,
                PostId = post.Id,
                ParentId = parentId,
                CreatedAt = DateTime.UtcNow
            };            

            var result = await _context.CommentEntities.AddAsync(comment);
            await _context.SaveChangesAsync();

            return result.Entity;
        }

        public async Task<PaginatedList<CommentEntity>> GetAllCommentOnCommentPaginatedList(CommentEntity comment, int pageNumber, int pageSize, bool includeRelated = false, bool includeMetric = false)
        {
            IQueryable<CommentEntity> query = _context.CommentEntities
                .AsNoTracking()
                .Where(c => c.ParentId == comment.Id);

            if (includeRelated)
            {
                query = query
                    .Include(c => c.ApplicationUser)
                    .Include(c => c.Post);
            }

            if (includeMetric)
            {
                query = query
                    .Include(c => c.CommentMetric);
            }

            return await PaginatedList<CommentEntity>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<CommentEntity> Update(CommentEntity comment, UpdateCommentDTO dto)
        {
            comment.Content = dto.Content;
            comment.UpdatedAt = DateTime.UtcNow;

            _context.Entry<CommentEntity>(comment).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return comment;
        }

    }
}