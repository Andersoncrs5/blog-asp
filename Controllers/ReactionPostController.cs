using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using blog.DTOs.ReactionPost;
using blog.utils.enums;
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
            string? id = User.FindFirst(ClaimTypes.Sid)?.Value;
            ApplicationUser user = await _uow.UserRepository.Get(id);
            PostEntity post = await _uow.PostRepository.Get(postId);

            bool exists = await _uow.ReactionPostRepository.Exists(user, post);
            
            return Ok(new Response(
                "success",
                exists? "Reaction already exists!": "Reaction are not exists!",
                200,
                exists
            ));
        }

        [HttpPost]
        [EnableRateLimiting("CreateItemPolicy")]
        public async Task<IActionResult> ToggleReaction([FromBody] ReactionPostDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            ApplicationUser? user = await _uow.UserRepository.Get(userId);

            PostEntity post = await _uow.PostRepository.Get(dto.PostId);
            
            ReactionPostResponse reactionResult = await _uow.ReactionPostRepository.ToggleReaction(user, post, dto.Action);

            UserMetricEntity userMetric = await _uow.UserMetricRepository.Get(user.Id);
            PostMetricEntity postMetric = await _uow.PostMetricRepository.Get(post);
            
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

            return Ok(new Response( 
                "success",
                responseMessage,
                200, 
                reactionResult.ReactionEntity 
            ));
        }

        [HttpDelete("{PostId:required}")]
        [EnableRateLimiting("DeleteItemPolicy")]
        public async Task<IActionResult> Remove(long PostId)
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            ApplicationUser user = await _uow.UserRepository.Get(userId);
            PostEntity post = await _uow.PostRepository.Get(PostId);
            PostMetricEntity metric = await _uow.PostMetricRepository.Get(post);
            ReactionPostEntity reaction = await _uow.ReactionPostRepository.Remove(user, post);

            PostMetricEntity metricUpdate = await _uow.PostMetricRepository.SumOrRedLikeOrDislike(metric, SumOrRedEnum.REDUCE ,reaction.Reaction);

            await _uow.PostRepository.CalculateEngagementScore(post, metricUpdate);

            return Ok(new Response(
                "success",
                "Reaction removed",
                200,
                null
            ));
        }

        [HttpGet("get-all-user")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfUserPaginated([FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        {
            string? id = User.FindFirst(ClaimTypes.Sid)?.Value;
            ApplicationUser user = await _uow.UserRepository.Get(id);
            PaginatedList<ReactionPostEntity> result = await _uow.ReactionPostRepository.GetAllOfUserPaginated(user, pageNumber, pageSize);
            result.Code = 200;

            return Ok(result);
        }

        [HttpGet("{postId:required}/get-all-post")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfPostPaginated(long postId, [FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        {
            PostEntity post = await _uow.PostRepository.Get(postId);
            PaginatedList<ReactionPostEntity> result = await _uow.ReactionPostRepository.GetAllOfPostPaginated(post, pageNumber, pageSize);
            result.Code = 200;
            
            return Ok(result);
        }

        [HttpGet("{userId:required}/get-all-user")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfUserPaginated(string userId, [FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        {
            ApplicationUser user = await _uow.UserRepository.Get(userId!);
            PaginatedList<ReactionPostEntity> result = await _uow.ReactionPostRepository.GetAllOfUserPaginated(user, pageNumber, pageSize);
            result.Code = 200;

            return Ok(result);
        }


    }
}