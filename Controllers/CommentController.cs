using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Blog.DTOs.Comment;
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
    public class CommentController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public CommentController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet("/{Id:required}/{includeRelated:bool?}/{includeMetric:bool?}")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> Get(ulong Id, bool includeRelated, bool includeMetric )
        {
            CommentEntity comment = await _uow.CommentRepository.Get(Id, includeRelated, includeMetric);

            return Ok(new Response(
                "success",
                "Comment found with successfully",
                200,
                comment
            ));
        }

        [HttpDelete("{Id:required}")]
        [EnableRateLimiting("DeleteItemPolicy")]
        public async Task<IActionResult> Delete(ulong Id)
        {
            CommentEntity comment = await _uow.CommentRepository.Get(Id, false);
            await _uow.CommentRepository.Delete(comment);

            return Ok(new Response(
                "success",
                "Comment deleted with successfully",
                200,
                comment
            ));
        }

        [HttpGet("get-all-user/{includeRelated:bool?}/{includeMetric:bool?}")]
        [EnableRateLimiting("DeleteItemPolicy")]
        public async Task<IActionResult> GetAllOfUserPaginatedList([FromQuery] int pageNumber, [FromQuery] int pageSize, bool includeMetric, bool includeRelated)
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            ApplicationUser user = await _uow.UserRepository.Get(userId);
            PaginatedList<CommentEntity> result = await _uow.CommentRepository.GetAllOfUserPaginatedList(user, pageNumber, pageSize, includeRelated,includeMetric);

            result.Code = 200;
            return Ok(result);
        }

        [HttpGet("{postId:required}/get-all-post")]
        [EnableRateLimiting("DeleteItemPolicy")]
        public async Task<IActionResult> GetAllOfUserPaginatedList(long postId, [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            PostEntity post = await _uow.PostRepository.Get(postId);
            PaginatedList<CommentEntity> result = await _uow.CommentRepository.GetAllOfPostPaginatedList(post, pageNumber, pageSize);

            result.Code = 200;
            return Ok(result);
        }

        [HttpGet("{commentId:required}/get-all-comment-on-comment/{includeRelated:bool?}/{includeMetric:bool?}")]
        [EnableRateLimiting("DeleteItemPolicy")]
        public async Task<IActionResult> GetAllCommentOnCommentPaginatedList(ulong commentId, [FromQuery] int pageNumber, [FromQuery] int pageSize, bool includeRelated = false, bool includeMetric = false)
        {
            CommentEntity comment = await _uow.CommentRepository.Get(commentId);
            PaginatedList<CommentEntity> result = await _uow.CommentRepository.GetAllCommentOnCommentPaginatedList(comment, pageNumber, pageSize, includeRelated, includeMetric);

            result.Code = 200;
            return Ok(result);
        }

        [HttpPost("{postId:required}/{parentId?}")]
        [EnableRateLimiting("CreateItemPolicy")]
        public async Task<IActionResult> Create([FromBody] CreateCommentDTO dto, long postId, ulong? parentId)
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            ApplicationUser user = await _uow.UserRepository.Get(userId);
            PostEntity post = await _uow.PostRepository.Get(postId);

            CommentEntity comment = await _uow.CommentRepository.Create(user, post, dto, parentId);

            return Ok(new Response(
                "success",
                "Comment created with successfully",
                201,
                comment
            ));
        }

        [HttpPut("{commentId:required}")]
        [EnableRateLimiting("UpdateItemPolicy")]
        public async Task<IActionResult> Update(ulong commentId, [FromBody] UpdateCommentDTO dto)
        {
            CommentEntity comment = await _uow.CommentRepository.Get(commentId);
            CommentEntity result = await _uow.CommentRepository.Update(comment, dto);

            return Ok(new Response(
                "success",
                "Comment updated with successfully",
                200,
                result
            ));   
        }
    }
}