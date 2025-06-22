using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blog.DTOs.Preference;
using blog.entities;
using blog.SetRepositories.IRepositories;
using Blog.Context;
using Blog.entities;
using Blog.utils;
using Microsoft.EntityFrameworkCore;

namespace blog.SetRepositories.Repositories
{
    public class UserPreferenceRepository: IUserPreferenceRepository
    {
        private readonly AppDbContext _context;

        public UserPreferenceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserPreferenceEntity> GetAsync(long Id)
        {
            if (long.IsNegative(Id) || Id == 0)
                throw new ResponseException("Preference id is required!!!");

            UserPreferenceEntity? prefer = await _context.UserPreferenceEntities.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == Id);

            if (prefer is null)
                throw new ResponseException("Preference not found!!!", 404);

            return prefer;
        }

        public async Task RemoveAsync(UserPreferenceEntity prefer)
        {
            _context.UserPreferenceEntities.Remove(prefer);
            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedList<UserPreferenceEntity>> GetAllOfUserPaginatedAsync(ApplicationUser user, int pageNumber, int pageSize, bool includeRelations = true)
        {
            IQueryable<UserPreferenceEntity> query = _context.UserPreferenceEntities.AsNoTracking()
                .Where(u => u.ApplicationUserId == user.Id);

            if (includeRelations)
            {
                query = query.Include(u => u.Category);
            }

            return await PaginatedList<UserPreferenceEntity>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<UserPreferenceEntity> SaveAsync(CreatePreferenceDTO dto, ApplicationUser user)
        {
            int check = await _context.UserPreferenceEntities.AsNoTracking()
                .CountAsync(u => u.CategoryId == dto.CategoryId);

            if (check > 0)
                throw new ResponseException("This category already was added!!", 409);

            UserPreferenceEntity prefer = new UserPreferenceEntity
            {
                CategoryId = dto.CategoryId,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _context.UserPreferenceEntities.AddAsync(prefer);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<bool> GetPreferenceByCategoryAsync(ApplicationUser user, long categoryId)
        {
            int check = await _context.UserPreferenceEntities.AsNoTracking()
                .CountAsync(u => u.CategoryId == categoryId && u.ApplicationUserId == user.Id);

            return check > 0;
        }

    }
}