using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Context;
using Blog.entities;
using Blog.SetRepositories.IRepositories;
using Blog.SetRepositories.Repositories;
using Blog.SetServices.IServices;
using Blog.SetServices.Services;
using Microsoft.AspNetCore.Identity;

namespace Blog.SetUnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private IUserRepository? _userRepository;
        private IUserMetricRepository? _userMetricRepository;
        private ICategoryRepository? _categoryRepository;
        private IPostRepository? _postRepository;
        private IPostMetricRepository? _postMetricRepository;
        private IFavoritePostRepository? _favoritePostRepository;
        private ICommentRepository? _commentRepository;
        private ICommentMetricRepository? _commentMetricRepository;
        private IFavoriteCommentRepository? _favoriteCommentRepository;
        private IReactionPostRepository? _reactionPostRepository;
        private IReactionCommentRepository? _reactionCommentRepository;
        private IPlaylistItemRepository? _playlistItemRepository;
        private IPlaylistRepository? _playlistRepository;
        private IRecoverAccountRepository _recoverAccountRepository;
        private IMediaPostRepository _mediaPostRepository;

        public UnitOfWork(
            AppDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IMediaPostRepository MediaPostRepository
            => _mediaPostRepository ??= new MediaPostRepository(_context);

        public IEmailService EmailService 
            => _emailService ??= new EmailService(_configuration);

        public IUserRepository UserRepository
            => _userRepository ??= new UserRepository(_context, _userManager);

        public IRecoverAccountRepository RecoverAccountRepository
            => _recoverAccountRepository ??= new RecoverAccountRepository(_context, _userManager, _emailService);

        public IUserMetricRepository UserMetricRepository
            => _userMetricRepository ??= new UserMetricRepository(_context);

        public ICategoryRepository CategoryRepository
            => _categoryRepository ??= new CategoryRepository(_context);

        public IPostRepository PostRepository
            => _postRepository ??= new PostRepository(_context);

        public IPostMetricRepository PostMetricRepository
            => _postMetricRepository ??= new PostMetricRepository(_context);

        public IFavoritePostRepository FavoritePostRepository
            => _favoritePostRepository ??= new FavoritePostRepository(_context);

        public IFavoriteCommentRepository FavoriteCommentRepository
            => _favoriteCommentRepository ??= new FavoriteCommentRepository(_context);

        public IReactionPostRepository ReactionPostRepository
            => _reactionPostRepository ??= new ReactionPostRepository(_context);

        public IReactionCommentRepository ReactionCommentRepository
            => _reactionCommentRepository ??= new ReactionCommentRepository(_context);

        public ICommentRepository CommentRepository
            => _commentRepository ??= new CommentRepository(_context);

        public ICommentMetricRepository CommentMetricRepository
            => _commentMetricRepository ??= new CommentMetricRepository(_context);

        public IPlaylistItemRepository PlaylistItemRepository
            => _playlistItemRepository ??= new PlaylistItemRepository(_context);

        public IPlaylistRepository PlaylistRepository
            => _playlistRepository ??= new PlaylistRepository(_context);

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
