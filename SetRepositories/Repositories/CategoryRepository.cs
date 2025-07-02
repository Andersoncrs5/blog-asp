using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Context;
using Blog.DTOs.Category;
using Blog.entities;
using Blog.SetRepositories.IRepositories;
using Blog.utils;
using Microsoft.EntityFrameworkCore;

namespace Blog.SetRepositories.Repositories
{
    public class CategoryRepository: ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CategoryEntity> Get(long Id)
        {
            if(long.IsNegative(Id))
                throw new ResponseException("Id is required");

            CategoryEntity? category = await _context.CategoryEntities.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id.Equals(Id));
            
            if (category == null) 
                throw new ResponseException("Category not found", 404);
            
            return category;
        }

        public async Task Delete(CategoryEntity category)
        {
            this._context.Remove(category);
            await this._context.SaveChangesAsync();
        }

        public async Task<List<CategoryEntity>> GetAll(bool IsActived = true) 
        {
            return await this._context.CategoryEntities.AsNoTracking()
                .Where(u => u.IsActived == IsActived)
                .ToListAsync();
        }

        public IQueryable<CategoryEntity> GetAll()
        {
            return _context.CategoryEntities.AsNoTracking();
        }

        public async Task<CategoryEntity> Create(CreateCategoryDTO dto, ApplicationUser user)
        {
            var check = _context.CategoryEntities.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Name.Contains(dto.Name));

            if (check != null)
                throw new ResponseException("category name exists!!", StatusCodes.Status409Conflict);

            CategoryEntity category = dto.toCategoryEntity();

            category.ApplicationUserId = user.Id;

            var created = await this._context.CategoryEntities.AddAsync(category);

            await this._context.SaveChangesAsync();
            return created.Entity;
        }

        public async Task<CategoryEntity> Update(CategoryEntity category, UpdateCategoryDTO dto)
        {
            category.Name = dto.Name;

            _context.Entry(category).State = EntityState.Modified;

            await this._context.SaveChangesAsync();
            return category;
        }

        public async Task<CategoryEntity> ChangeStatusActive(CategoryEntity category)
        {
            category.IsActived = !category.IsActived;

            _context.Entry(category).State = EntityState.Modified;
            await this._context.SaveChangesAsync();

            return category;
        }

    }
}