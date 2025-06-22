using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using blog.DTOs.ReactionComment;
using Blog.entities;
using Blog.SetUnitOfWork;
using Blog.utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace blog.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ReactionCommentController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public ReactionCommentController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpPost]
        [EnableRateLimiting("CreateItemPolicy")]
        public async Task<IActionResult> Reaction([FromBody] CreateReactionCommentDTO dto )
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            ApplicationUser user = await _uow.UserRepository.Get(userId);
            CommentEntity comment = await _uow.CommentRepository.Get(dto.CommentId);
            ReactionCommentEntity? reaction = await _uow.ReactionCommentRepository.Reaction(comment, user, dto.action);

            return Ok(new Response(
                "success",
                reaction == null? "Reaction removed!": "Reaction added!",
                200,
                reaction
            ));
        }

        [HttpDelete("CommentId:required")]
        [EnableRateLimiting("DeleteItemPolicy")]
        public async Task<IActionResult> Remove(ulong CommentId)
        {
            CommentEntity comment = await _uow.CommentRepository.Get(CommentId);
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            ApplicationUser user = await _uow.UserRepository.Get(userId);
            await _uow.ReactionCommentRepository.Remove(comment, user);

            return Ok(new Response(
                "success",
                "Reaction removed",
                200,
                null
            ));
        }

        [HttpGet("exists/{commentId:required:long}")]
        [EnableRateLimiting("CheckExistsPolicy")]
        public async Task<IActionResult> Exists(ulong commentId)
        {
            string? id = User.FindFirst(ClaimTypes.Sid)?.Value;
            ApplicationUser user = await _uow.UserRepository.Get(id);
            CommentEntity comment = await _uow.CommentRepository.Get(commentId);

            bool exists = await _uow.ReactionCommentRepository.Exists(user, comment);
            
            return Ok(new Response(
                "success",
                exists? "Reaction already exists!": "Reaction are not exists!",
                200,
                exists
            ));
        }

        [HttpGet("get-all-user")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfUserPaginated([FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        {
            string? id = User.FindFirst(ClaimTypes.Sid)?.Value;
            ApplicationUser user = await _uow.UserRepository.Get(id);
            PaginatedList<ReactionCommentEntity> result = await _uow.ReactionCommentRepository.GetAllOfUserPaginated(user, pageNumber, pageSize);
            result.Code = 200;

            return Ok(result);
        }

        [HttpGet("{userId:required}/get-all-user")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfUserAnotherPaginated(string userId, [FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        {
            ApplicationUser user = await _uow.UserRepository.Get(userId);
            PaginatedList<ReactionCommentEntity> result = await _uow.ReactionCommentRepository.GetAllOfUserPaginated(user, pageNumber, pageSize);
            result.Code = 200;

            return Ok(result);
        }

        [HttpGet("{commentId:required}/get-all-comment")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfCommentPaginated(ulong commentId, [FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        {
            CommentEntity comment = await _uow.CommentRepository.Get(commentId);
            PaginatedList<ReactionCommentEntity> result = await _uow.ReactionCommentRepository.GetAllOfCommentPaginated(comment, pageNumber, pageSize);
            result.Code = 200;
            
            return Ok(result);
        }

    }
}