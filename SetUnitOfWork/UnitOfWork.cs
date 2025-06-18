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
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager; // UserManager é uma exceção, geralmente injetado diretamente

        // Campos privados para os repositórios, inicializados apenas na primeira requisição
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

        // O construtor agora só aceita as dependências básicas (DbContext, UserManager)
        public UnitOfWork(
            AppDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            // Repositórios não são inicializados aqui, mas nas propriedades (lazy)
        }

        // As propriedades agora usam a inicialização lazy para criar as instâncias
        // caso elas ainda não existam.
        // Desta forma, os repositórios só são criados quando realmente são acessados.

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

        public IFavoritePostRepository FavoritePostRepository
            => _favoritePostRepository ??= new FavoritePostRepository(_context);

        // CORREÇÃO: Nome da propriedade no UnitOfWork
        public IFavoriteCommentRepository FavoriteCommentRepository
            => _favoriteCommentRepository ??= new FavoriteCommentRepository(_context);

        public IReactionPostRepository ReactionPostRepository
            => _reactionPostRepository ??= new ReactionPostRepository(_context);

        public ICommentRepository CommentRepository
            => _commentRepository ??= new CommentRepository(_context);

        public ICommentMetricRepository CommentMetricRepository
            => _commentMetricRepository ??= new CommentMetricRepository(_context);

        /// <summary>
        /// Salva todas as mudanças pendentes no contexto do banco de dados.
        /// </summary>
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
