using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using blog.utils.Responses;
using Blog.DTOs.Post;
using Blog.entities;
using Blog.SetUnitOfWork;
using Blog.utils;
using Blog.utils.Filters.FiltersDTO;
using Blog.utils.Filters.FiltersQuerys;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Blog.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public PostController(IUnitOfWork uow)
        {
            _uow = uow;
        }
    
        [HttpGet("{Id:long:required}")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> Get(long Id)
        {
            if (Id <= 0) 
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

            PostEntity? post = await _uow.PostRepository.Get(Id);
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

            await _uow.PostMetricRepository.SumOrRedViewed(postMetric, Blog.utils.enums.SumOrRedEnum.SUM);

            return StatusCode(200, new ResponseBody<PostEntity>
            {
                Body = post,
                Code = 200,
                Message = "Post found with success",
                Status = true,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpDelete("{Id:long:required}")]
        [EnableRateLimiting("DeleteItemPolicy")]
        public async Task<IActionResult> Delete(long Id)
        {
            if (Id <= 0) 
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

            PostEntity? post = await _uow.PostRepository.Get(Id);
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

            await _uow.PostRepository.Delete(post, user);

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

            await _uow.UserMetricRepository.SumOrRedPostsCount(metric, utils.enums.SumOrRedEnum.SUM);
            
            return StatusCode(200, new ResponseBody<PostEntity>
            {
                Body = null,
                Code = 200,
                Message = "Post deleted with successfully",
                Status = true,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpPost]
        [EnableRateLimiting("CreateItemPolicy")]
        public async Task<IActionResult> Create([FromBody] CreatePostDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            CategoryEntity? category = await _uow.CategoryRepository.Get(dto.categoryId);
            if (category == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Datetime = DateTimeOffset.Now,
                    Message = "Category not found",
                    Status = false
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

            PostEntity postCreated = await _uow.PostRepository.Create(user, dto.MappearToPostEntity(), category);

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

            await _uow.UserMetricRepository.SumOrRedPostsCount(metric, utils.enums.SumOrRedEnum.SUM);
            await _uow.PostMetricRepository.Create(postCreated);
            await _uow.NotificationRepository.SendNotificationToFollowersAboutNewPost(user, postCreated);

            return StatusCode(200, new ResponseBody<PostEntity>
            {
                Body = null,
                Code = 200,
                Message = "Post created with successfully",
                Status = true,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("get-all-user-paginated")] 
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfUserPaginated([FromQuery] PostFilterDTO filter)
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

            IQueryable<PostEntity> query = _uow.PostRepository.GetAllOfUser(user, filter.IsActived);

            query = PostQueryFilter.ApplyFilters(query, filter);

            PaginatedList<PostEntity> result = await PaginatedList<PostEntity>.CreateAsync(query, filter.PageNumber, filter.PageSize);            
            
            return Ok(new ResponseBody<PaginatedList<PostEntity>>
            {
                Status = true,
                Message = "All post",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("{userId:required}/get-all-user-paginated")] 
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfAnotherUserPaginated(string userId, [FromQuery] PostFilterDTO filter)
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
     
            IQueryable<PostEntity> query = _uow.PostRepository.GetAllOfUser(user, filter.IsActived);

            query = PostQueryFilter.ApplyFilters(query, filter);

            PaginatedList<PostEntity> result = await PaginatedList<PostEntity>.CreateAsync(query, filter.PageNumber, filter.PageSize);            
            
            
            return Ok(new ResponseBody<PaginatedList<PostEntity>>
            {
                Status = true,
                Message = "All post",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }
        
        [HttpGet("get-all-to-me-paginated")] 
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllToMePaginated([FromQuery] PostFilterDTO filter)
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

            IQueryable<PostEntity> query = await _uow.PostRepository.GetAllToMe(user);

            query = PostQueryFilter.ApplyFilters(query, filter);

            PaginatedList<PostEntity> result = await PaginatedList<PostEntity>.CreateAsync(query, filter.PageNumber, filter.PageSize);   
            
            return Ok(new ResponseBody<PaginatedList<PostEntity>>
            {
                Status = true,
                Message = "All post",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("get-all")] 
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAll([FromQuery] PostFilterDTO filter)
        {    
            IQueryable<PostEntity> query = _uow.PostRepository.GetAll();

            filter.IsActived = true;
            query = PostQueryFilter.ApplyFilters(query, filter);

            PaginatedList<PostEntity> result = await PaginatedList<PostEntity>.CreateAsync(query, filter.PageNumber, filter.PageSize);   
            
            return Ok(new ResponseBody<PaginatedList<PostEntity>>
            {
                Status = true,
                Message = "All post",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpPut("{PostId:required:long}")]
        [EnableRateLimiting("UpdateItemPolicy")]
        public async Task<IActionResult> Update(long PostId, [FromBody] UpdatePostDTO dto)
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

            PostEntity result = await _uow.PostRepository.Update(post, dto, user);

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

            await _uow.PostMetricRepository.SumOrRedEditedCount(postMetric, Blog.utils.enums.SumOrRedEnum.SUM);

            return Ok(new ResponseBody<PostEntity>
            {
                Status = true,
                Message = "Post updated with success",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("change-status/{Id:long:required}")]
        [EnableRateLimiting("UpdateItemPolicy")]
        public async Task<IActionResult> ChangeStatusActive(long Id)
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

            if (Id <= 0) 
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
            
            PostEntity? post = await _uow.PostRepository.Get(Id);
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
            
            PostEntity result = await _uow.PostRepository.ChangeStatusActive(post, user);

            return Ok(new ResponseBody<PostEntity>
            {
                Status = true,
                Message = "Status post changed",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("{Id:long:required}/get-metric")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetMetric(long Id)
        {
            if (Id <= 0) 
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

            PostEntity? post = await _uow.PostRepository.Get(Id);
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

            return Ok(new ResponseBody<PostMetricEntity>
            {
                Status = true,
                Message = "Post metric found with successfully",
                Code = 200,
                Body = metric,
                Datetime = DateTimeOffset.Now
            });
        }

        

    }
}