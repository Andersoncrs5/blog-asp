using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
            CommentEntity comment = await _uow.CommentRepository.Get(commentId);
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;

            ApplicationUser user = await _uow.UserRepository.Get(userId);

            FavoriteCommentEntity result = await _uow.FavoriteCommentRepository.Save(user, comment);

            UserMetricEntity metric = await _uow.UserMetricRepository.Get(user.Id);
            await _uow.UserMetricRepository.SumOrRedSavedCommentsCount(metric, Blog.utils.enums.SumOrRedEnum.SUM);

            return Ok(new Response(
                "success",
                "",
                200,
                result
            ));
        }

        [HttpDelete("{Id:required}")]
        [EnableRateLimiting("SaveOrRemoveFavoriteItemPolicy")]
        public async Task<IActionResult> Remove(ulong Id)
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            ApplicationUser user = await _uow.UserRepository.Get(userId);

            FavoriteCommentEntity favorite = await _uow.FavoriteCommentRepository.Get(Id);
            await _uow.FavoriteCommentRepository.Remove(favorite);

            UserMetricEntity metric = await _uow.UserMetricRepository.Get(user.Id);
            await _uow.UserMetricRepository.SumOrRedSavedCommentsCount(metric, Blog.utils.enums.SumOrRedEnum.REDUCE);

            return Ok(new Response(
                "success",
                "Comment removed with favorite",
                200,
                favorite
            ));
        }

        [HttpGet("get-all-user")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfUserPaginated([FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            ApplicationUser user = await _uow.UserRepository.Get(userId);

            PaginatedList<FavoriteCommentEntity> result = await _uow.FavoriteCommentRepository.GetAllOfUserPaginated(user, pageNumber, pageSize);
            result.Code = 200;

            return Ok(result);
        }

        [HttpGet("{userId:required}/get-all-user")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfAnotherUserPaginated(string userId, [FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        {
            ApplicationUser user = await _uow.UserRepository.Get(userId);

            PaginatedList<FavoriteCommentEntity> result = await _uow.FavoriteCommentRepository.GetAllOfUserPaginated(user, pageNumber, pageSize);
            result.Code = 200;

            return Ok(result);
        }

        [HttpGet("{Id:required}/get-all-comment")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfCommentPaginated(ulong Id, [FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        {
            CommentEntity comment = await _uow.CommentRepository.Get(Id);
            PaginatedList<FavoriteCommentEntity> result = await _uow.FavoriteCommentRepository.GetAllOfCommentPaginated(comment, pageNumber, pageSize);
            result.Code = 200;

            return Ok(result);
        }

        [HttpGet("{Id:required}/exists")]
        [EnableRateLimiting("CheckExistsPolicy")]
        public async Task<IActionResult> Exists(ulong Id)
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            ApplicationUser user = await _uow.UserRepository.Get(userId);
            CommentEntity comment = await _uow.CommentRepository.Get(Id);
            bool check = await _uow.FavoriteCommentRepository.Exists(user, comment);

            return Ok(new Response(
                "success",
                check? "Comment are save how favorite!": "Comment are not save how favorite!",
                200,
                check
            ));
        }

    }
}