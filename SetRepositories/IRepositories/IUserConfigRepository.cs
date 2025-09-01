using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blog.DTOs.UserConfig;
using blog.entities;
using Blog.entities;

namespace blog.SetRepositories.IRepositories
{
    public interface IUserConfigRepository
    {
        Task<UserConfigEntity?> GetAsync(ApplicationUser user);
        Task DeleteAsync(UserConfigEntity config);
        Task<bool> Exists(string userId);
        Task<UserConfigEntity> CreateAsync(CreateUserConfigDTO dto ,ApplicationUser user);
        Task<UserConfigEntity> UpdateAsync(UpdateUserConfigDTO dto ,ApplicationUser user, UserConfigEntity config);

    }
}