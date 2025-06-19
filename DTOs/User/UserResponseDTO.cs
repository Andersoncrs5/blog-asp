using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.DTOs.User
{
    public class UserResponseDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public UserResponseDTO(string id, string name, string email)
        {
            Id = id;
            Name = name;
            Email = email;
        }
    }
}