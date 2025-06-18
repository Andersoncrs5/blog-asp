using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.SetRepositories.IRepositories;

namespace Blog.SetUnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IUserMetricRepository UserMetricRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IPostRepository PostRepository { get; }
        IPostMetricRepository PostMetricRepository { get; }
        IFavoritePostRepository FavoritePostRepository { get; }
        IFavoriteCommentRepository FavoriteCommentRepository { get; }
        IReactionPostRepository ReactionPostRepository { get; }
        ICommentRepository CommentRepository { get; }
        ICommentMetricRepository CommentMetricRepository { get; }
        IReactionCommentRepository ReactionCommentRepository { get; }
        IPlaylistItemRepository PlaylistItemRepository { get; }
        IPlaylistRepository PlaylistRepository { get; }

        Task Commit();
    }
}