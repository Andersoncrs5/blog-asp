using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using blog.DTOs.ReactionPost;
using blog.utils.enums;
using blog.utils.Responses;
using blog.utils.Responses.ReactionPost;
using Blog.entities;
using Blog.SetUnitOfWork;
using Blog.utils;
using Blog.utils.enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace blog.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ReactionPostController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public ReactionPostController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet("exists/{postId:required:long}")]
        [EnableRateLimiting("CheckExistsPolicy")]
        public async Task<IActionResult> Exists(long postId)
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

            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (string.IsNullOrWhiteSpace(userId)) {
                return Unauthorized(new ResponseBody<string>
                {
                    Body = null,
                    Code = 401,
                    Message = "You are not authorizetion",
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

            bool exists = await _uow.ReactionPostRepository.Exists(user, post);
            
            return Ok(new ResponseBody<bool>
            {
                Status = true,
                Message = exists? "Reaction already exists!": "Reaction are not exists!",
                Code = 200,
                Body = exists,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpPost]
        [EnableRateLimiting("CreateItemPolicy")]
        public async Task<IActionResult> ToggleReaction([FromBody] ReactionPostDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (string.IsNullOrWhiteSpace(userId)) {
                return Unauthorized(new ResponseBody<string>
                {
                    Body = null,
                    Code = 401,
                    Message = "You are not authorizetion",
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

            PostEntity? post = await _uow.PostRepository.Get(dto.PostId);
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
            
            ReactionPostResponse reactionResult = await _uow.ReactionPostRepository.ToggleReaction(user, post, dto.Action);

            UserMetricEntity? userMetric = await _uow.UserMetricRepository.Get(user.Id);
            if (userMetric == null)
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

            PostMetricEntity? postMetric = await _uow.PostMetricRepository.Get(post);
            if (postMetric == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Message = "Post metric not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }
            
            switch (reactionResult.ChangeType)
            {
                case ReactionPostChangeType.Added:
                    if (reactionResult.NewReaction.HasValue)
                    {
                        await _uow.UserMetricRepository.SumOrRedLikesOrDislikeGivenCountInPost(userMetric, SumOrRedEnum.SUM, reactionResult.NewReaction.Value);
                        var metricUpdate = await _uow.PostMetricRepository.SumOrRedLikeOrDislike(postMetric, SumOrRedEnum.SUM, reactionResult.NewReaction.Value);
                        await _uow.PostRepository.CalculateEngagementScore(post, metricUpdate);
                    }
                    break;

                case ReactionPostChangeType.Removed:
                    
                    if (reactionResult.OldReaction.HasValue)
                    {
                        await _uow.UserMetricRepository.SumOrRedLikesOrDislikeGivenCountInPost(userMetric, SumOrRedEnum.REDUCE, reactionResult.OldReaction.Value);
                        var metricUpdate = await _uow.PostMetricRepository.SumOrRedLikeOrDislike(postMetric, SumOrRedEnum.REDUCE, reactionResult.OldReaction.Value);
                        await _uow.PostRepository.CalculateEngagementScore(post, metricUpdate!);
                    }
                    break;

                case ReactionPostChangeType.Updated:
                    
                    
                    if (reactionResult.OldReaction.HasValue)
                    {
                        
                        await _uow.UserMetricRepository.SumOrRedLikesOrDislikeGivenCountInPost(userMetric, SumOrRedEnum.REDUCE, reactionResult.OldReaction.Value);
                        
                    }
                    
                    if (reactionResult.NewReaction.HasValue)
                    {
                        
                        await _uow.UserMetricRepository.SumOrRedLikesOrDislikeGivenCountInPost(userMetric, SumOrRedEnum.SUM, reactionResult.NewReaction.Value);
                    }
                    break;
            }

            
            string responseMessage = reactionResult.ChangeType switch
            {
                ReactionPostChangeType.Added => "Reaction added!",
                ReactionPostChangeType.Removed => "Reaction removed!",
                ReactionPostChangeType.Updated => "Reaction updated!",
                _ => "Reaction processed."
            };

            return Ok(new ResponseBody<ReactionPostEntity>
            {
                Status = true,
                Message = responseMessage,
                Code = 200,
                Body = reactionResult.ReactionEntity,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpDelete("{PostId:required}")]
        [EnableRateLimiting("DeleteItemPolicy")]
        public async Task<IActionResult> Remove(long PostId)
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (PostId <= 0) 
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

            if (string.IsNullOrWhiteSpace(userId)) {
                return Unauthorized(new ResponseBody<string>
                {
                    Body = null,
                    Code = 401,
                    Message = "You are not authorizetion",
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

            PostEntity? post = await _uow.PostRepository.Get(PostId);
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
            
            PostMetricEntity? metric = await _uow.PostMetricRepository.Get(post);
            if (metric == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Message = "Post metric not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            ReactionPostEntity reaction = await _uow.ReactionPostRepository.Remove(user, post);
            PostMetricEntity metricUpdate = await _uow.PostMetricRepository.SumOrRedLikeOrDislike(metric, SumOrRedEnum.REDUCE ,reaction.Reaction);

            await _uow.PostRepository.CalculateEngagementScore(post, metricUpdate);

            return Ok(new ResponseBody<ReactionPostEntity>
            {
                Status = true,
                Message = "Reaction removed",
                Code = 200,
                Body = null,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("get-all-user")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfUserPaginated([FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (string.IsNullOrWhiteSpace(userId)) {
                return Unauthorized(new ResponseBody<string>
                {
                    Body = null,
                    Code = 401,
                    Message = "You are not authorizetion",
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

            PaginatedList<ReactionPostEntity> result = await _uow.ReactionPostRepository.GetAllOfUserPaginated(user, pageNumber, pageSize);
            
            return Ok(new ResponseBody<PaginatedList<ReactionPostEntity>>
            {
                Status = true,
                Message = "All Reaction Post",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("{postId:required}/get-all-post")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfPostPaginated(long postId, [FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
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
            
            PaginatedList<ReactionPostEntity> result = await _uow.ReactionPostRepository.GetAllOfPostPaginated(post, pageNumber, pageSize);
            
            return Ok(new ResponseBody<PaginatedList<ReactionPostEntity>>
            {
                Status = true,
                Message = "All Reaction Post",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("{userId:required}/get-all-user")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfUserPaginated(string userId, [FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(userId)) {
                return Unauthorized(new ResponseBody<string>
                {
                    Body = null,
                    Code = 401,
                    Message = "You are not authorizetion",
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

            PaginatedList<ReactionPostEntity> result = await _uow.ReactionPostRepository.GetAllOfUserPaginated(user, pageNumber, pageSize);
            
            return Ok(new ResponseBody<PaginatedList<ReactionPostEntity>>
            {
                Status = true,
                Message = "All Reaction Post",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }


    }
}