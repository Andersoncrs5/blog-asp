using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            PostEntity post = await this._uow.PostRepository.Get(postId);
            ApplicationUser user = await this._uow.UserRepository.Get(post.ApplicationUserId);
            await this._uow.PostRepository.Delete(post, user);

            return Ok(new Response(
                "success",
                "Post deleted with successfully",
                201,
                null
            ));
        }

        [HttpDelete("delete-comment/{commentId:required}")]
        [Authorize(Roles = "AdminRole, SuperAdminRole")]
        public async Task<IActionResult> DeleteComment(ulong commentId)
        {
            CommentEntity comment = await this._uow.CommentRepository.Get(commentId);
            await this._uow.CommentRepository.Delete(comment);

            return Ok(new Response(
                "success",
                "Comment deleted with successfully",
                201,
                null
            ));
        }

        [HttpPost("{userId:required}")]
        [Authorize(Roles = "SuperAdminRole")]
        public async Task<IActionResult> AddRoleAdmInUser(string userId)
        {
            ApplicationUser user = await _uow.UserRepository.Get(userId);
            IdentityRole? role = await _roleManager.FindByNameAsync("AdminRole");

            if (role == null || string.IsNullOrEmpty(role.Name))
                return Ok(new ResponseException(
                    "Role not found",
                    StatusCodes.Status404NotFound
                ));

            if (await _userManager.IsInRoleAsync(user, role.Name))
                return BadRequest(new ResponseException($"User '{user.UserName}' does have the '{role.Name}' role.", 400));

            var result = await _userManager.AddToRoleAsync(user, role.Name);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new ResponseException(
                    "Error removing AdminRole from user.",
                    StatusCodes.Status500InternalServerError,
                    "fail",
                    errors
                ));
            }

            return Ok(new Response(
                "success",
                $"The user {user.UserName} now is a adm!!!!! ",
                StatusCodes.Status201Created,
                null
            ));
        }

        [HttpDelete("remove-role-adm/{userId:required}")]
        [Authorize(Roles = "SuperAdminRole")]
        public async Task<IActionResult> RemoveRoleAdmInUser(string userId)
        {
            ApplicationUser user = await _uow.UserRepository.Get(userId);
            IdentityRole? role = await _roleManager.FindByNameAsync("AdminRole");

            if (role == null || string.IsNullOrEmpty(role.Name))
                return Ok(new ResponseException(
                    "Role not found",
                    StatusCodes.Status404NotFound
                ));

            if (!await _userManager.IsInRoleAsync(user, role.Name))
                return BadRequest(new ResponseException($"User '{user.UserName}' does not have the '{role.Name}' role.", 400));
            

            var result = await _userManager.RemoveFromRoleAsync(user, role.Name);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new ResponseException(
                    "Error removing AdminRole from user.",
                    StatusCodes.Status500InternalServerError,
                    "fail",
                    errors
                ));
            }

            return Ok(new Response(
                "success",
                $"Role adm removed of user {user.UserName}!",
                StatusCodes.Status201Created,
                null
            ));
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdminRole")]
        public async Task<IActionResult> GetAllAdm()
        {
            IdentityRole? role = await _roleManager.FindByNameAsync("AdminRole");

            if (role == null || string.IsNullOrEmpty(role.Name))
                return Ok(new ResponseException(
                    "Role not found",
                    StatusCodes.Status404NotFound
                ));

            IList<ApplicationUser>? result = await _userManager.GetUsersInRoleAsync(role.Name);

            return Ok(new Response(
                "success",
                "All user has role adm listed!",
                StatusCodes.Status201Created,
                result
            ));
        }

        [HttpDelete("delete-user/{userId:required}")]
        [Authorize(Roles = "SuperAdminRole")]
        public async Task<IActionResult> DeleteUser(string userId) 
        {
            ApplicationUser user = await _uow.UserRepository.Get(userId);
            await _uow.UserRepository.Delete(user);

            return Ok(new Response(
                "success",
                $"The user {user.UserName} deleted! ",
                StatusCodes.Status201Created,
                null
            ));
        }


    }
}