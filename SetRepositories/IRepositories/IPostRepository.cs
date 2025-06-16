using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.entities;

namespace Blog.SetRepositories.IRepositories
{
    public interface IPostRepository
    {
        Task<PostEntity> Get(long Id);
        Task<PostEntity> Delete(PostEntity post);
        Task<List<PostEntity>> GetAllOfUser(ApplicationUser user, int pageNumber, int pageSize);

    }
}