using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blog.DTOs.UserConfig;
using blog.entities;
using blog.SetRepositories.IRepositories;
using Blog.Context;
using Blog.entities;
using Blog.utils;
using Microsoft.EntityFrameworkCore;

namespace blog.SetRepositories.Repositories
{
    public class UserConfigRepository: IUserConfigRepository
    {
        private readonly AppDbContext _context;

        public UserConfigRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserConfigEntity> GetAsync(ApplicationUser user, bool includeRelations = false)
        {
            UserConfigEntity? config = await _context.UserConfigEntities.AsNoTracking()
                .FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);

            if (config is null)
                throw new ResponseException("Config not found", 404);

            return config;
        }

        public async Task DeleteAsync(UserConfigEntity config)
        {
            _context.UserConfigEntities.Remove(config);
            await _context.SaveChangesAsync();
        }

        public async Task<UserConfigEntity> CreateAsync(CreateUserConfigDTO dto, ApplicationUser user)
        {
            bool exists = await _context.UserConfigEntities.AnyAsync(uc => uc.ApplicationUserId == user.Id);
            if (exists)
                throw new ResponseException("User config already exists for this user.", 409);
                
            UserConfigEntity newConfig = new UserConfigEntity
            {
                ApplicationUserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            _context.Entry(newConfig).CurrentValues.SetValues(dto);
            var result = await _context.UserConfigEntities.AddAsync(newConfig);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<UserConfigEntity> UpdateAsync(UpdateUserConfigDTO dto ,ApplicationUser user)
        {
            UserConfigEntity? config = await _context.UserConfigEntities
                .FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);

            if (config is null)
                throw new ResponseException("Config not found", 404);

            _context.Entry(config).CurrentValues.SetValues(dto);
            config.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return config;
        }

    }
}