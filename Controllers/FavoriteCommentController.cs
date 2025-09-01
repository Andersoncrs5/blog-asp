using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using blog.utils.Responses;
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
    public class FavoriteCommentController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public FavoriteCommentController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpPost("{commentId:required}")]
        [EnableRateLimiting("SaveOrRemoveFavoriteItemPolicy")]
        public async Task<IActionResult> Save(ulong commentId)
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

            bool exists = await _uow.FavoriteCommentRepository.CheckExistsCommentWithFavorite(userId, comment.Id);

            if (exists == true)
            {
                return Conflict(new ResponseBody<string>
                {
                    Body = null,
                    Code = 409,
                    Message = "Comment already are saved with favorite",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            FavoriteCommentEntity result = await _uow.FavoriteCommentRepository.Save(user, comment);

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

            await _uow.UserMetricRepository.SumOrRedSavedCommentsCount(metric, Blog.utils.enums.SumOrRedEnum.SUM);

            return Ok(new ResponseBody<FavoriteCommentEntity>
            {
                Status = true,
                Message = "Comment favorited",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpDelete("{Id:required}")]
        [EnableRateLimiting("SaveOrRemoveFavoriteItemPolicy")]
        public async Task<IActionResult> Remove(ulong Id)
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

            FavoriteCommentEntity? favorite = await _uow.FavoriteCommentRepository.Get(Id);
            if (favorite == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Message = "Favorite comment not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            await _uow.FavoriteCommentRepository.Remove(favorite);

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

            await _uow.UserMetricRepository.SumOrRedSavedCommentsCount(metric, Blog.utils.enums.SumOrRedEnum.REDUCE);

            return Ok(new ResponseBody<FavoriteCommentEntity>
            {
                Status = true,
                Message = "Comment removed with favorite",
                Code = 200,
                Body = favorite,
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

            PaginatedList<FavoriteCommentEntity> result = await _uow.FavoriteCommentRepository.GetAllOfUserPaginated(user, pageNumber, pageSize);

            return Ok(new ResponseBody<PaginatedList<FavoriteCommentEntity>>
            {
                Status = true,
                Message = "Favorite Comments found",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("{userId:required}/get-all-user")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfAnotherUserPaginated(string userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
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

            PaginatedList<FavoriteCommentEntity> result = await _uow.FavoriteCommentRepository.GetAllOfUserPaginated(user, pageNumber, pageSize);

            return Ok(new ResponseBody<PaginatedList<FavoriteCommentEntity>>
            {
                Status = true,
                Message = "Favorite Comments found",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("{Id:required}/get-all-comment")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfCommentPaginated(ulong Id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            CommentEntity? comment = await _uow.CommentRepository.Get(Id);

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

            PaginatedList<FavoriteCommentEntity> result = await _uow.FavoriteCommentRepository.GetAllOfCommentPaginated(comment, pageNumber, pageSize);

            return Ok(result);
        }

        [HttpGet("{Id:required}/exists")]
        [EnableRateLimiting("CheckExistsPolicy")]
        public async Task<IActionResult> Exists(ulong Id)
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

            CommentEntity? comment = await _uow.CommentRepository.Get(Id);

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

            bool check = await _uow.FavoriteCommentRepository.Exists(user, comment);

            return Ok(new ResponseBody<bool>
            {
                Status = true,
                Message = check ? "Comment are save how favorite!" : "Comment are not save how favorite!",
                Code = 200,
                Body = check,
                Datetime = DateTimeOffset.Now
            });
        }

    }
}