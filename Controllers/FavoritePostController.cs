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

            PaginatedList<FavoritePostEntity> result = await _uow.FavoritePostRepository.GetAllOfUserPaginated(user, pageNumber, pageSize);
            
            return Ok(new ResponseBody<PaginatedList<FavoritePostEntity>>
            {
                Status = true,
                Message = "Favorite Posts found",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("{Id:required}/get-all-another-user")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllAnotherOfUserPaginated(string Id, [FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
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

            PaginatedList<FavoritePostEntity> result = await _uow.FavoritePostRepository.GetAllOfUserPaginated(user, pageNumber, pageSize);
            
            return Ok(new ResponseBody<PaginatedList<FavoritePostEntity>>
            {
                Status = true,
                Message = "Favorite Posts found",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("{postId:required:long}/get-all-post")]
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

            PaginatedList<FavoritePostEntity> result = await _uow.FavoritePostRepository.GetAllOfPostPaginated(post, pageNumber, pageSize);
            
            return Ok(new ResponseBody<PaginatedList<FavoritePostEntity>>
            {
                Status = true,
                Message = "Favorite Posts found",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpPost("{postId:required:long}/save")]
        [EnableRateLimiting("SaveOrRemoveFavoriteItemPolicy")]
        public async Task<IActionResult> Save(long postId)
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

            bool exists = await _uow.FavoritePostRepository.CheckExistsPostWithFavorite(userId, postId);
            if (exists == true)
            {
                return Conflict(new ResponseBody<string>
                {
                    Body = null,
                    Code = 409,
                    Message = "Post already are saved with favorite",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            FavoritePostEntity save = await _uow.FavoritePostRepository.Save(user, post);

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

            await _uow.UserMetricRepository.SumOrRedSavedPostsCount(metric, Blog.utils.enums.SumOrRedEnum.SUM);

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

            PostMetricEntity metricUpdate = await _uow.PostMetricRepository.SumOrRedCommentCount(postMetric, Blog.utils.enums.SumOrRedEnum.SUM);

            await _uow.PostRepository.CalculateEngagementScore(post, metricUpdate);

            return Ok(new ResponseBody<FavoritePostEntity>
            {
                Status = true,
                Message = "Post favorited with successfully",
                Code = 201,
                Body = save,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpDelete("{saveId:long:required}/remove")]
        [EnableRateLimiting("SaveOrRemoveFavoriteItemPolicy")]
        public async Task<IActionResult> Remove(long saveId)
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
            
            FavoritePostEntity? save = await _uow.FavoritePostRepository.Get(saveId);
            if (save == null) 
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Message = "Favorite Post not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            await _uow.FavoritePostRepository.Remove(save);

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

            await _uow.UserMetricRepository.SumOrRedSavedPostsCount(metric, Blog.utils.enums.SumOrRedEnum.REDUCE);

            if (save.PostId <= 0) 
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

            PostEntity? post = await _uow.PostRepository.Get(save.PostId);
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

            PostMetricEntity metricUpdate =  await _uow.PostMetricRepository.SumOrRedCommentCount(postMetric, Blog.utils.enums.SumOrRedEnum.REDUCE);

            await _uow.PostRepository.CalculateEngagementScore(post, metricUpdate);

            return Ok(new ResponseBody<FavoritePostEntity>
            {
                Status = true,
                Message = "Post removed with successfully",
                Code = 201,
                Body = save,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("{postId:required:long}/exists")]
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

            bool check = await _uow.FavoritePostRepository.Exists(user, post);

            return Ok(new ResponseBody<bool>
            {
                Status = true,
                Message = check? "Post are save how favorite!": "Post are not save how favorite!",
                Code = 201,
                Body = check,
                Datetime = DateTimeOffset.Now
            });
        }

    }
}