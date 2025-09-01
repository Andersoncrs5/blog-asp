using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blog.utils.Filters.FiltersDTO;
using blog.utils.Filters.FiltersQuerys;
using blog.utils.Responses;
using Blog.entities;
using Blog.SetUnitOfWork;
using Blog.utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace blog.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [EnableRateLimiting("AdmSystemPolicy")]
    public class AdmController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdmController(IUnitOfWork uow, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _uow = uow;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpDelete("delete-post/{postId:required:long}")]
        [Authorize(Roles = "AdminRole, SuperAdminRole")]
        public async Task<IActionResult> DeletePost(long postId)
        {
            if (postId <= 0)
            {
                return BadRequest(new ResponseBody<string>
                {
                    Body = null,
                    Code = 400,
                    Message = "Post id is required",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            PostEntity? post = await _uow.PostRepository.Get(postId);

            if (post == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Message = "Post not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            ApplicationUser? user = await _uow.UserRepository.Get(post.ApplicationUserId);

            if (user == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 400,
                    Message = "User not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            await _uow.PostRepository.Delete(post, user);

            return Ok(new ResponseBody<string>
            {
                Body = null,
                Code = 200,
                Datetime = DateTimeOffset.Now,
                Message = "Post deleted",
                Status = true
            });
        }

        [HttpDelete("delete-comment/{commentId:required}")]
        [Authorize(Roles = "AdminRole, SuperAdminRole")]
        public async Task<IActionResult> DeleteComment(ulong commentId)
        {
            CommentEntity? comment = await this._uow.CommentRepository.Get(commentId);

            if (comment == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 400,
                    Message = "Comment not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            await this._uow.CommentRepository.Delete(comment);

            return Ok(new ResponseBody<string>
            {
                Body = null,
                Code = 200,
                Datetime = DateTimeOffset.Now,
                Message = "comment deleted",
                Status = true
            });
        }

        [HttpPost("{userId:required}")]
        [Authorize(Roles = "SuperAdminRole")]
        public async Task<IActionResult> AddRoleAdmInUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest(new ResponseBody<string>
                {
                    Body = null,
                    Code = 400,
                    Message = "Id is required",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            ApplicationUser? user = await _uow.UserRepository.Get(userId);

            if (user == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Message = "User not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            IdentityRole? role = await _roleManager.FindByNameAsync("AdminRole");

            if (role == null || string.IsNullOrEmpty(role.Name))
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Message = "Role not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });

            if (await _userManager.IsInRoleAsync(user, role.Name))
                return BadRequest(new ResponseBody<string>
                {
                    Body = null,
                    Code = 400,
                    Message = $"User '{user.UserName}' have the '{role.Name}' role.",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });

            var result = await _userManager.AddToRoleAsync(user, role.Name);

            if (!result.Succeeded)
            {
                IEnumerable<string>? errors = result.Errors.Select(e => e.Description);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBody<IEnumerable<string>>
                {
                    Message = "Error adding AdminRole from user.",
                    Code = StatusCodes.Status500InternalServerError,
                    Status = false,
                    Body = errors,
                    Datetime = DateTimeOffset.Now
                });
            }

            return Ok(new ResponseBody<string>
            {
                Message = $"The user {user.UserName} now is a adm!!!!! ",
                Code = StatusCodes.Status200OK,
                Body = null,
                Datetime = DateTimeOffset.Now,
                Status = false
            });
        }

        [HttpDelete("remove-role-adm/{userId:required}")]
        [Authorize(Roles = "SuperAdminRole")]
        public async Task<IActionResult> RemoveRoleAdmInUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest(new ResponseBody<string>
                {
                    Body = null,
                    Code = 400,
                    Message = "Id is required",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            ApplicationUser? user = await _uow.UserRepository.Get(userId);

            if (user == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Message = "User not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            IdentityRole? role = await _roleManager.FindByNameAsync("AdminRole");

            if (role == null || string.IsNullOrEmpty(role.Name))
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Message = "Role not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });

            if (!await _userManager.IsInRoleAsync(user, role.Name))
                return BadRequest(new ResponseBody<string>
                {
                    Body = null,
                    Code = 400,
                    Message = $"User '{user.UserName}' does have the '{role.Name}' role.",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });

            var result = await _userManager.RemoveFromRoleAsync(user, role.Name);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBody<IEnumerable<string>>
                {
                    Message = "Error removing AdminRole from user.",
                    Code = StatusCodes.Status500InternalServerError,
                    Status = false,
                    Body = errors,
                    Datetime = DateTimeOffset.Now
                });
            }

            return Ok(new ResponseBody<string>
            {
                Body = null,
                Message = $"Role: {role.Name} adm removed of user {user.UserName}!",
                Code = StatusCodes.Status200OK,
                Datetime = DateTimeOffset.Now,
                Status = false
            });
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdminRole")]
        public async Task<IActionResult> GetAllAdm()
        {
            IdentityRole? role = await _roleManager.FindByNameAsync("AdminRole");

            if (role == null || string.IsNullOrEmpty(role.Name))
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Message = "Role not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });

            IList<ApplicationUser>? result = await _userManager.GetUsersInRoleAsync(role.Name);

            return Ok(new ResponseBody<IList<ApplicationUser>>
            {
                Status = true,
                Message = "All user has role adm listed!",
                Code = StatusCodes.Status200OK,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpDelete("delete-user/{userId:required}")]
        [Authorize(Roles = "SuperAdminRole")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest(new ResponseBody<string>
                {
                    Body = null,
                    Code = 400,
                    Message = "Id is required",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            ApplicationUser? user = await _uow.UserRepository.Get(userId);

            if (user == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Message = "User not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            await _uow.UserRepository.Delete(user);

            return Ok(new ResponseBody<string>
            {
                Status = true,
                Message = $"The user {user.UserName} was deleted!",
                Code = StatusCodes.Status200OK,
                Body = null,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("get-all-category")]
        [Authorize(Roles = "AdminRole, SuperAdminRole")]
        public async Task<IActionResult> GetAllCategory([FromQuery] CategoryFilterDTO filter)
        {
            IQueryable<CategoryEntity> query = _uow.CategoryRepository.GetAll();

            PaginatedList<CategoryEntity> result = await PaginatedList<CategoryEntity>.CreateAsync(query, filter.PageNumber, filter.PageSize);

            return Ok(result);
        }

    }
}