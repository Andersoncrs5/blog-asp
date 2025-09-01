using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using blog.DTOs.ReactionComment;
using blog.utils.enums;
using blog.utils.Responses;
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
            if (string.IsNullOrWhiteSpace(userId))
            {
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

            CommentEntity? comment = await _uow.CommentRepository.Get(dto.CommentId);
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

            ReactionCommentResponse reactionResult = await _uow.ReactionCommentRepository.Reaction(comment, user, dto.Action);

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


            return Ok(new ResponseBody<ReactionCommentEntity>
            {
                Status = true,
                Message = responseMessage,
                Code = 200,
                Body = reactionResult.ReactionEntity,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpDelete("{Id:required}")]
        [EnableRateLimiting("DeleteItemPolicy")]
        public async Task<IActionResult> Remove(ulong Id)
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
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

            ReactionCommentEntity? result = await _uow.ReactionCommentRepository.Remove(Id);
            if (result == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Message = "Reaction Comment not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            await _uow.UserMetricRepository.SumOrRedLikesOrDislikeGivenCountInComment(userMetric, SumOrRedEnum.REDUCE, result.Reaction);

            return Ok(new ResponseBody<ReactionCommentEntity>
            {
                Status = true,
                Message = "Reaction removed",
                Code = 200,
                Body = null,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("exists/{commentId:required:long}")]
        [EnableRateLimiting("CheckExistsPolicy")]
        public async Task<IActionResult> Exists(ulong commentId)
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
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

            bool exists = await _uow.ReactionCommentRepository.Exists(user, comment);

            return Ok(new ResponseBody<bool>
            {
                Status = true,
                Message = exists ? "Reaction already exists!" : "Reaction are not exists!",
                Code = 200,
                Body = exists,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("get-all-user")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfUserPaginated([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
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

            PaginatedList<ReactionCommentEntity> result = await _uow.ReactionCommentRepository.GetAllOfUserPaginated(user, pageNumber, pageSize);

            return Ok(new ResponseBody<PaginatedList<ReactionCommentEntity>>
            {
                Status = true,
                Message = "All Reaction Comment",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("{userId:required}/get-all-user")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfUserAnotherPaginated(string userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
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

            PaginatedList<ReactionCommentEntity> result = await _uow.ReactionCommentRepository.GetAllOfUserPaginated(user, pageNumber, pageSize);

            return Ok(new ResponseBody<PaginatedList<ReactionCommentEntity>>
            {
                Status = true,
                Message = "All Reaction Comment",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("{commentId:required}/get-all-comment")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfCommentPaginated(ulong commentId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
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

            PaginatedList<ReactionCommentEntity> result = await _uow.ReactionCommentRepository.GetAllOfCommentPaginated(comment, pageNumber, pageSize);

            return Ok(new ResponseBody<PaginatedList<ReactionCommentEntity>>
            {
                Status = true,
                Message = "All Reaction Comment",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

    }
}