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
    public class FavoritePostController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public FavoritePostController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet("get-all-user")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfUserPaginated([FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            ApplicationUser user = await _uow.UserRepository.Get(userId);

            PaginatedList<FavoritePostEntity> result = await _uow.FavoritePostRepository.GetAllOfUserPaginated(user, pageNumber, pageSize);
            result.Code = 200;

            return Ok(result);
        }

        [HttpGet("{Id:required}/get-all-another-user")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllAnotherOfUserPaginated(string Id, [FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        {
            ApplicationUser user = await _uow.UserRepository.Get(Id);

            PaginatedList<FavoritePostEntity> result = await _uow.FavoritePostRepository.GetAllOfUserPaginated(user, pageNumber, pageSize);
            result.Code = 200;

            return Ok(result);
        }

        [HttpGet("{postId:required:long}/get-all-post")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfPostPaginated(long postId, [FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        {
            PostEntity post = await _uow.PostRepository.Get(postId);

            PaginatedList<FavoritePostEntity> result = await _uow.FavoritePostRepository.GetAllOfPostPaginated(post, pageNumber, pageSize);
            result.Code = 200;

            return Ok(result);
        }

        [HttpPost("{postId:required:long}/save")]
        [EnableRateLimiting("SaveOrRemoveFavoriteItemPolicy")]
        public async Task<IActionResult> Save(long postId)
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            ApplicationUser user = await _uow.UserRepository.Get(userId);

            PostEntity post = await _uow.PostRepository.Get(postId);
            FavoritePostEntity save = await _uow.FavoritePostRepository.Save(user, post);

            UserMetricEntity metric = await _uow.UserMetricRepository.Get(user.Id);
            await _uow.UserMetricRepository.SumOrRedSavedPostsCount(metric, Blog.utils.enums.SumOrRedEnum.SUM);

            PostMetricEntity postMetric = await _uow.PostMetricRepository.Get(post);
            PostMetricEntity metricUpdate = await _uow.PostMetricRepository.SumOrRedCommentCount(postMetric, Blog.utils.enums.SumOrRedEnum.SUM);

            await _uow.PostRepository.CalculateEngagementScore(post, metricUpdate);

            return Ok(new Response(
                "success",
                "",
                201,
                save
            ));
        }

        [HttpDelete("{saveId:long:required}/remove")]
        [EnableRateLimiting("SaveOrRemoveFavoriteItemPolicy")]
        public async Task<IActionResult> Remove(long saveId)
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            ApplicationUser user = await _uow.UserRepository.Get(userId);
            
            FavoritePostEntity save = await _uow.FavoritePostRepository.Get(saveId);
            await _uow.FavoritePostRepository.Remove(save);

            UserMetricEntity metric = await _uow.UserMetricRepository.Get(user.Id);
            await _uow.UserMetricRepository.SumOrRedSavedPostsCount(metric, Blog.utils.enums.SumOrRedEnum.REDUCE);

            PostEntity post = await _uow.PostRepository.Get(save.PostId);
            PostMetricEntity postMetric = await _uow.PostMetricRepository.Get(post);
            PostMetricEntity metricUpdate =  await _uow.PostMetricRepository.SumOrRedCommentCount(postMetric, Blog.utils.enums.SumOrRedEnum.REDUCE);

            await _uow.PostRepository.CalculateEngagementScore(post, metricUpdate);

            return Ok(new Response(
                "success",
                "Favorite removed!",
                200,
                null
            ));
        }

        [HttpGet("{postId:required:long}/exists")]
        [EnableRateLimiting("CheckExistsPolicy")]
        public async Task<IActionResult> Exists(long postId)
        {
            PostEntity post = await _uow.PostRepository.Get(postId);
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            ApplicationUser user = await _uow.UserRepository.Get(userId);

            bool check = await _uow.FavoritePostRepository.Exists(user, post);
            
            return Ok(new Response(
                "success",
                check? "Post are save how favorite!": "Post are not save how favorite!",
                200,
                check
            ));
        }

    }
}