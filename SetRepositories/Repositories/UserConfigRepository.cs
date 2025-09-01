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

        public async Task<UserConfigEntity?> GetAsync(ApplicationUser user)
        {
            UserConfigEntity? config = await _context.UserConfigEntities.AsNoTracking()
                .FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);

            if (config is null)
                return null;

            return config;
        }

        public async Task DeleteAsync(UserConfigEntity config)
        {
            _context.UserConfigEntities.Remove(config);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> Exists(string userId)
        {
            return await _context.UserConfigEntities.AnyAsync(uc => uc.ApplicationUserId == userId);
        }

        public async Task<UserConfigEntity> CreateAsync(CreateUserConfigDTO dto, ApplicationUser user)
        {       
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

        public async Task<UserConfigEntity> UpdateAsync(UpdateUserConfigDTO dto ,ApplicationUser user, UserConfigEntity config)
        {
            _context.Entry(config).CurrentValues.SetValues(dto);
            config.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return config;
        }

    }
}