using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blog.DTOs.Preference;
using blog.entities;
using Blog.entities;
using Blog.utils;

namespace blog.SetRepositories.IRepositories
{
    public interface IUserPreferenceRepository
    {
        Task<UserPreferenceEntity> GetAsync(long Id);
        Task RemoveAsync(UserPreferenceEntity prefer);
        Task<PaginatedList<UserPreferenceEntity>> GetAllOfUserPaginatedAsync(ApplicationUser user, int pageNumber, int pageSize, bool includeRelations = true);
        Task<UserPreferenceEntity> SaveAsync(CreatePreferenceDTO dto, ApplicationUser user);
        Task<bool> GetPreferenceByCategoryAsync(ApplicationUser user, long categoryId);

    }
}