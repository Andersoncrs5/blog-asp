using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using blog.DTOs.ReactionComment;
using blog.utils.enums;
using blog.utils.Responses.ReactionComment;
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
        public async Task<IActionResult> Reaction([FromBody] CreateReactionCommentDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value; 

            ApplicationUser user = await _uow.UserRepository.Get(userId);
            CommentEntity comment = await _uow.CommentRepository.Get(dto.CommentId);

            ReactionCommentResponse reactionResult = await _uow.ReactionCommentRepository.Reaction(comment, user, dto.Action);

            UserMetricEntity userMetric = await _uow.UserMetricRepository.Get(user.Id);

            switch (reactionResult.ChangeType)
            {
                case ReactionCommentChangeType.Added:
                    if (reactionResult.NewReaction.HasValue)
                    {
                        await _uow.UserMetricRepository.SumOrRedLikesOrDislikeGivenCountInComment(userMetric, SumOrRedEnum.SUM, reactionResult.NewReaction.Value);
                    }
                    break;

                case ReactionCommentChangeType.Removed:
                    if (reactionResult.OldReaction.HasValue)
                    {
                        await _uow.UserMetricRepository.SumOrRedLikesOrDislikeGivenCountInComment(userMetric, SumOrRedEnum.REDUCE, reactionResult.OldReaction.Value);
                    }
                    break;

                case ReactionCommentChangeType.Updated:
                    if (reactionResult.OldReaction.HasValue)
                    {
                        await _uow.UserMetricRepository.SumOrRedLikesOrDislikeGivenCountInComment(userMetric, SumOrRedEnum.REDUCE, reactionResult.OldReaction.Value);
                    }
                    if (reactionResult.NewReaction.HasValue)
                    {
                        await _uow.UserMetricRepository.SumOrRedLikesOrDislikeGivenCountInComment(userMetric, SumOrRedEnum.SUM, reactionResult.NewReaction.Value);
                    }
                    break;
            }

            string responseMessage = reactionResult.ChangeType switch
            {
                ReactionCommentChangeType.Added => "Reaction added!",
                ReactionCommentChangeType.Removed => "Reaction removed!",
                ReactionCommentChangeType.Updated => "Reaction updated!",
                _ => "Reaction processed."
            };

            return Ok(new Response( 
                "success",
                responseMessage,
                200, 
                reactionResult.ReactionEntity 
            ));
        }

        [HttpDelete("{Id:required}")]
        [EnableRateLimiting("DeleteItemPolicy")]
        public async Task<IActionResult> Remove(ulong Id)
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value; 

            ApplicationUser user = await _uow.UserRepository.Get(userId);

            UserMetricEntity userMetric = await _uow.UserMetricRepository.Get(user.Id);

            var result = await _uow.ReactionCommentRepository.Remove(Id);
            await _uow.UserMetricRepository.SumOrRedLikesOrDislikeGivenCountInComment(userMetric, SumOrRedEnum.REDUCE, result.Reaction);

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