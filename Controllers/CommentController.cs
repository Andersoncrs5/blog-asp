using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using blog.utils.Filters.FiltersDTO;
using blog.utils.Filters.FiltersQuerys;
using blog.utils.Responses;
using Blog.DTOs.Comment;
using Blog.entities;
using Blog.SetUnitOfWork;
using Blog.utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

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

        [HttpGet("{Id:required}/{includeRelated:bool?}/{includeMetric:bool?}")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> Get(ulong Id, bool includeRelated, bool includeMetric)
        {
            CommentEntity? comment = await _uow.CommentRepository.Get(Id, includeRelated, includeMetric);

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

            return Ok(new ResponseBody<CommentEntity>
            {
                Status = true,
                Message = "Comment found with successfully",
                Code = 200,
                Body = comment,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpDelete("{Id:required}")]
        [EnableRateLimiting("DeleteItemPolicy")]
        public async Task<IActionResult> Delete(ulong Id)
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
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

            CommentEntity? comment = await _uow.CommentRepository.Get(Id, false);
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

            PostEntity? post = await _uow.PostRepository.Get(comment.PostId);
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

            await _uow.CommentRepository.Delete(comment);

            UserMetricEntity? metric = await _uow.UserMetricRepository.Get(user.Id);
            if (metric == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Datetime = DateTimeOffset.Now,
                    Message = "User metric not found",
                    Status = false
                });
            }

            await _uow.UserMetricRepository.SumOrRedCommentsCount(metric, Blog.utils.enums.SumOrRedEnum.REDUCE);

            PostMetricEntity postMetric = await _uow.PostMetricRepository.Get(post);
            await _uow.PostMetricRepository.SumOrRedCommentCount(postMetric, Blog.utils.enums.SumOrRedEnum.REDUCE);

            return Ok(new ResponseBody<string>
            {
                Status = true,
                Message = "Comment deleted with successfully",
                Code = 200,
                Body = null,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("get-all-user")]
        [EnableRateLimiting("DeleteItemPolicy")]
        public async Task<IActionResult> GetAllOfUserPaginatedList([FromQuery] CommentFilterDTO filter)
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
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

            IQueryable<CommentEntity> query = _uow.CommentRepository.GetAllOfUser(user);

            PaginatedList<CommentEntity> result = await PaginatedList<CommentEntity>.CreateAsync(query, filter.PageNumber, filter.PageSize);

            return Ok(new ResponseBody<PaginatedList<CommentEntity>>
            {
                Status = true,
                Message = "Comments found",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("get-all-user/{userId:required}")]
        [EnableRateLimiting("DeleteItemPolicy")]
        public async Task<IActionResult> GetAllOfAnotherUserPaginatedList(string userId, [FromQuery] CommentFilterDTO filter)
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

            IQueryable<CommentEntity> query = _uow.CommentRepository.GetAllOfUser(user);

            PaginatedList<CommentEntity> result = await PaginatedList<CommentEntity>.CreateAsync(query, filter.PageNumber, filter.PageSize);

            return Ok(new ResponseBody<PaginatedList<CommentEntity>>
            {
                Status = true,
                Message = "Comments found",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("{postId:required}/get-all-post")]
        [EnableRateLimiting("DeleteItemPolicy")]
        public async Task<IActionResult> GetAllOfPostPaginatedList(long postId, [FromQuery] CommentFilterDTO filter)
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

            IQueryable<CommentEntity> query = _uow.CommentRepository.GetAllOfPost(post);

            PaginatedList<CommentEntity> result = await PaginatedList<CommentEntity>.CreateAsync(query, filter.PageNumber, filter.PageSize);

            return Ok(new ResponseBody<PaginatedList<CommentEntity>>
            {
                Status = true,
                Message = "Comments found",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("{commentId:required}/get-all-comment-on-comment/{includeRelated:bool?}/{includeMetric:bool?}")]
        [EnableRateLimiting("DeleteItemPolicy")]
        public async Task<IActionResult> GetAllCommentOnCommentPaginatedList(ulong commentId, [FromQuery] int pageNumber, [FromQuery] int pageSize, bool includeRelated = false, bool includeMetric = false)
        {
            if (commentId <= 0)
            {
                return BadRequest(new ResponseBody<string>
                {
                    Body = null,
                    Code = 400,
                    Message = "Comment id is required",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            CommentEntity? comment = await _uow.CommentRepository.Get(commentId);
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

            PaginatedList<CommentEntity> result = await _uow.CommentRepository.GetAllCommentOnCommentPaginatedList(comment, pageNumber, pageSize, includeRelated, includeMetric);

            return Ok(new ResponseBody<PaginatedList<CommentEntity>>
            {
                Status = true,
                Message = "Comments found",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpPost("{postId:required}")]
        [EnableRateLimiting("CreateItemPolicy")]
        [SwaggerOperation(Summary = "Creates a new comment for a post, optionally as a reply to another comment.")]
        public async Task<IActionResult> Create(
            [FromBody] CreateCommentDTO dto,
            long postId,
            [FromQuery] ulong? parentId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;

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

            CommentEntity comment = await _uow.CommentRepository.Create(user, post, dto, parentId);

            UserMetricEntity? metric = await _uow.UserMetricRepository.Get(user.Id);
            if (metric == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Datetime = DateTimeOffset.Now,
                    Message = "User metric not found",
                    Status = false
                });
            }

            await _uow.UserMetricRepository.SumOrRedCommentsCount(metric, Blog.utils.enums.SumOrRedEnum.SUM);

            PostMetricEntity postMetric = await _uow.PostMetricRepository.Get(post);
            await _uow.PostMetricRepository.SumOrRedCommentCount(postMetric, Blog.utils.enums.SumOrRedEnum.SUM);

            await _uow.CommentMetricRepository.Create(comment);

            return Ok(new ResponseBody<CommentEntity>
            {
                Status = true,
                Message = "Comment created with successfully",
                Code = 201,
                Body = comment,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpPut("{commentId:required}")]
        [EnableRateLimiting("UpdateItemPolicy")]
        public async Task<IActionResult> Update(ulong commentId, [FromBody] UpdateCommentDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            CommentEntity? comment = await _uow.CommentRepository.Get(commentId);
            if (comment == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Message = "Comment not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            CommentEntity result = await _uow.CommentRepository.Update(comment, dto);

            return Ok(new ResponseBody<CommentEntity>
            {
                Status = true,
                Message = "Comment updated with successfully",
                Code = 201,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("{commentId:required}/get-metric")]
        public async Task<IActionResult> GetMetric(ulong commentId)
        {
            CommentEntity? comment = await _uow.CommentRepository.Get(commentId);

            if (comment == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Message = "Comment not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            CommentMetricEntity? metric = await _uow.CommentMetricRepository.Get(comment);

            if (metric == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Message = "Comment Metric not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            return Ok(new ResponseBody<CommentMetricEntity>
            {
                Status = true,
                Message = "Comment metric found with successfully",
                Code = 200,
                Body = metric,
                Datetime = DateTimeOffset.Now
            });
        }

    }
}