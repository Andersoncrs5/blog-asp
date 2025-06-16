using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Context;
using Blog.entities;
using Blog.SetRepositories.IRepositories;
using Blog.SetRepositories.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Blog.SetUnitOfWork
{
    public class UnitOfWork: IUnitOfWork, IDisposable
    {
        private readonly  AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private IUserRepository _userRepository;
        private IUserMetricRepository _userMetricRepository;
        private ICategoryRepository _categoryRepository;

        public UnitOfWork(
            AppDbContext context,
            UserManager<ApplicationUser> userManager, 
            IUserMetricRepository userMetricRepository, 
            ICategoryRepository categoryRepository
            )
        {
            _categoryRepository = categoryRepository;
            _userMetricRepository = userMetricRepository;
            _context = context;
            _userManager = userManager;
        }

        public IUserRepository UserRepository 
            => _userRepository ??= new UserRepository(_context, _userManager);

        public IUserMetricRepository UserMetricRepository
            => _userMetricRepository ??= new UserMetricRepository(_context);

        public ICategoryRepository CategoryRepository
            => _categoryRepository ??= new CategoryRepository(_context);

        public async Task Commit()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}