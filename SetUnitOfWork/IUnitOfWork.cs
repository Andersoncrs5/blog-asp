using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.SetRepositories.IRepositories;

namespace Blog.SetUnitOfWork
{
    public interface IUnitOfWork: IDisposable
    {
        IUserRepository UserRepository { get; }
        IUserMetricRepository UserMetricRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IPostRepository PostRepository { get; }
        IPostMetricRepository PostMetricRepository { get; }
        Task Commit();
    }
}