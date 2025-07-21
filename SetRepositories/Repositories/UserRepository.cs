using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Context;
using Blog.DTOs.User;
using Blog.entities;
using Blog.SetRepositories.IRepositories;
using Blog.utils;
using Microsoft.AspNetCore.Identity;

namespace Blog.SetRepositories.Repositories
{
    public class UserRepository: IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ApplicationUser> Get(string? id, bool includeMetric = false) 
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ResponseException("User Id is required", 400, "fail");

            ApplicationUser? user = await _userManager.FindByIdAsync(id);

            if (user == null)
                throw new ResponseException("User not found", 404, "fail");

            return user;
        }

        public async Task Delete(ApplicationUser user) 
        {
            await _userManager.DeleteAsync(user);
        }

        public async Task<ApplicationUser> Update(ApplicationUser? user, UpdateUserDto dto) 
        {
            if (user == null)
                throw new ResponseException("User is required", 400, "fail");

            user.UserName = dto.Name.Trim();

            if(!string.IsNullOrWhiteSpace(dto.Password)) 
            {
                PasswordHasher<ApplicationUser>? passwordHasher = new PasswordHasher<ApplicationUser>();
                user.PasswordHash = passwordHasher.HashPassword(user, dto.Password);
            }

            IdentityResult? result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new ResponseException("Update failed", 500, "failed");
            }

            return user;
        }
    }
}