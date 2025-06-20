using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Blog.DTOs.Category;
using Blog.entities;

namespace Blog.SetRepositories.IRepositories
{
    public interface ICategoryRepository
    {
        Task<CategoryEntity> Get(long Id);
        Task<CategoryEntity> Create(CreateCategoryDTO dto, ApplicationUser user);
        Task<List<CategoryEntity>> GetAll(bool IsActived = true);
        Task Delete(CategoryEntity category);
        Task<CategoryEntity> Update(CategoryEntity category, UpdateCategoryDTO dto);
        Task<CategoryEntity> ChangeStatusActive(CategoryEntity category);
    }
}