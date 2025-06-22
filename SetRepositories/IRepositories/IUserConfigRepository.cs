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
        Task<UserConfigEntity> GetAsync(ApplicationUser user, bool includeRelations = false);
        Task DeleteAsync(UserConfigEntity config);
        Task<UserConfigEntity> CreateAsync(CreateUserConfigDTO dto ,ApplicationUser user);
        Task<UserConfigEntity> UpdateAsync(UpdateUserConfigDTO dto ,ApplicationUser user);


    }
}