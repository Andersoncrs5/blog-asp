using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.DTOs.User;
using Blog.entities;

namespace Blog.SetRepositories.IRepositories
{
    public interface IUserRepository
    {
        Task<ApplicationUser> Get(string id);
        Task Delete(ApplicationUser? user);
        Task<ApplicationUser> Update(ApplicationUser? user, UpdateUserDto dto);
    }
}