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
        private IPostRepository _postRepository;
        private IPostMetricRepository _postMetricRepository;

        public UnitOfWork(
            AppDbContext context,
            UserManager<ApplicationUser> userManager, 
            IUserMetricRepository userMetricRepository, 
            ICategoryRepository categoryRepository,
            IPostRepository postRepository,
            IPostMetricRepository postMetricRepository
            )
        {
            _categoryRepository = categoryRepository;
            _userMetricRepository = userMetricRepository;
            _context = context;
            _userManager = userManager;
            _postRepository = postRepository;
            _postMetricRepository = postMetricRepository;
        }

        public IUserRepository UserRepository 
            => _userRepository ??= new UserRepository(_context, _userManager);

        public IUserMetricRepository UserMetricRepository
            => _userMetricRepository ??= new UserMetricRepository(_context);

        public ICategoryRepository CategoryRepository
            => _categoryRepository ??= new CategoryRepository(_context);

        public IPostRepository PostRepository 
            => _postRepository ??= new PostRepository(_context);

        public IPostMetricRepository PostMetricRepository 
            => _postMetricRepository ??= new PostMetricRepository(_context);

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