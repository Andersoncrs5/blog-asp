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

        public async Task<CategoryEntity?> Get(long Id)
        {
            if(long.IsNegative(Id))
                throw new ArgumentNullException(nameof(Id));

            CategoryEntity? category = await _context.CategoryEntities.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id.Equals(Id));
            
            if (category == null)
                return null;

            return category;
        }

        public async Task Delete(CategoryEntity category)
        {
            _context.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CategoryEntity>> GetAll(bool IsActived = true) 
        {
            return await _context.CategoryEntities.AsNoTracking()
                .Where(u => u.IsActived == IsActived)
                .ToListAsync();
        }

        public IQueryable<CategoryEntity> GetAll()
        {
            return _context.CategoryEntities.AsNoTracking();
        }

        public async Task<bool> ExistsByName(string name)
        {
            return await _context.CategoryEntities
                .AsNoTracking()
                .AnyAsync(c => c.Name.Contains(name));
        }

        public async Task<CategoryEntity> Create(CreateCategoryDTO dto, ApplicationUser user)
        {
            CategoryEntity category = dto.toCategoryEntity();

            category.ApplicationUserId = user.Id;

            var created = await _context.CategoryEntities.AddAsync(category);

            await _context.SaveChangesAsync();
            return created.Entity;
        }

        public async Task<CategoryEntity> Update(CategoryEntity category, UpdateCategoryDTO dto)
        {
            category.Name = dto.Name;

            _context.Entry(category).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<CategoryEntity> ChangeStatusActive(CategoryEntity category)
        {
            category.IsActived = !category.IsActived;

            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return category;
        }

    }
}