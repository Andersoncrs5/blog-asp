using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
            PostEntity post = await _uow.PostRepository.Get(Id);
            
            PostMetricEntity postMetric = await _uow.PostMetricRepository.Get(post);
            await _uow.PostMetricRepository.SumOrRedViewed(postMetric, Blog.utils.enums.SumOrRedEnum.SUM);

            return Ok(new Response(
                "success",
                "Post founded with success",
                200,
                post
            ));
        }

        [HttpDelete("{Id:long:required}")]
        [EnableRateLimiting("DeleteItemPolicy")]
        public async Task<IActionResult> Delete(long Id)
        {
            PostEntity post = await _uow.PostRepository.Get(Id);
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            ApplicationUser user = await _uow.UserRepository.Get(userId);
            await _uow.PostRepository.Delete(post, user);

            UserMetricEntity metric = await _uow.UserMetricRepository.Get(user.Id);
            await _uow.UserMetricRepository.SumOrRedPostsCount(metric, utils.enums.SumOrRedEnum.SUM);
            
            return Ok(new Response(
                "success",
                "Post deleted with success",
                200,
                null
            ));
        }

        [HttpPost]
        [EnableRateLimiting("CreateItemPolicy")]
        public async Task<IActionResult> Create([FromBody] CreatePostDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            CategoryEntity category = await _uow.CategoryRepository.Get(dto.categoryId);
            ApplicationUser user = await _uow.UserRepository.Get(userId);
            PostEntity postCreated = await _uow.PostRepository.Create(user, dto.MappearToPostEntity(), category);

            UserMetricEntity metric = await _uow.UserMetricRepository.Get(user.Id);
            await _uow.UserMetricRepository.SumOrRedPostsCount(metric, utils.enums.SumOrRedEnum.SUM);

            await _uow.PostMetricRepository.Create(postCreated);

            await _uow.NotificationRepository.SendNotificationToFollowersAboutNewPost(user, postCreated);

            return Ok(new Response(
                "success",
                "Post created with successfully",
                201,
                postCreated
            ));
        }

        [HttpGet("get-all-user-paginated/{canFilter:bool}")] 
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfUserPaginated([FromQuery] PostFilterDTO filter, bool canFilter = false)
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value; 
            ApplicationUser user = await _uow.UserRepository.Get(userId);

            IQueryable<PostEntity> query = _uow.PostRepository.GetAllOfUser(user, filter.IsActived);

            if (canFilter == true){query = PostQueryFilter.ApplyFilters(query, filter);}

            var result = await PaginatedList<PostEntity>.CreateAsync(query, filter.PageNumber, filter.PageSize);            
            result.Code = 200;
            return Ok(result);
        }

        [HttpGet("{userId:required}/get-all-user-paginated")] 
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfAnotherUserPaginated(string userId, [FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        { 
            ApplicationUser user = await _uow.UserRepository.Get(userId);
            
            IQueryable<PostEntity> query = _uow.PostRepository.GetAllOfUser(user);
            var result = await PaginatedList<PostEntity>.CreateAsync(query, pageNumber, pageSize);            
            result.Code = 200;
            return Ok(result);
        }
        
        [HttpGet("get-all-to-me-paginated/{canFilter:bool?}")] 
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllToMePaginated([FromQuery] PostFilterDTO filter, bool canFilter = false)
        { 
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            ApplicationUser user = await _uow.UserRepository.Get(userId);

            IQueryable<PostEntity> query = await _uow.PostRepository.GetAllToMe(user);

            if (canFilter == true){query = PostQueryFilter.ApplyFilters(query, filter);}

            PaginatedList<PostEntity> result = await PaginatedList<PostEntity>.CreateAsync(query, filter.PageNumber, filter.PageSize);   
            
            result.Code = 200;
            return Ok(result);
        }

        [HttpGet("get-all/{canFilter:bool}")] 
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAll([FromQuery] PostFilterDTO filter, bool canFilter = false)
        {    
            IQueryable<PostEntity> query = _uow.PostRepository.GetAll();

            filter.IsActived = true;
            if (canFilter == true){query = PostQueryFilter.ApplyFilters(query, filter);}

            PaginatedList<PostEntity> result = await PaginatedList<PostEntity>.CreateAsync(query, filter.PageNumber, filter.PageSize);   
            
            result.Code = 200;
            return Ok(result);
        }

        [HttpPut("{PostId:required:long}")]
        [EnableRateLimiting("UpdateItemPolicy")]
        public async Task<IActionResult> Update(long PostId, [FromBody] UpdatePostDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            ApplicationUser user = await _uow.UserRepository.Get(userId);
            PostEntity post = await _uow.PostRepository.Get(PostId);

            PostEntity result = await _uow.PostRepository.Update(post, dto, user);

            PostMetricEntity postMetric = await _uow.PostMetricRepository.Get(post);
            await _uow.PostMetricRepository.SumOrRedEditedCount(postMetric, Blog.utils.enums.SumOrRedEnum.SUM);

            return Ok(new Response(
                "success",
                "Post updated with success",
                200,
                result
            ));
        }

        [HttpGet("change-status/{Id:long:required}")]
        [EnableRateLimiting("UpdateItemPolicy")]
        public async Task<IActionResult> ChangeStatusActive(long Id)
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            ApplicationUser user = await _uow.UserRepository.Get(userId);
            PostEntity post = await _uow.PostRepository.Get(Id);
            PostEntity result = await _uow.PostRepository.ChangeStatusActive(post, user);

            return Ok(new Response(
                "success",
                "Status changed!",
                200,
                result
            ));
        }

        [HttpGet("{Id:long:required}/get-metric")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetMetric(long Id)
        {
            PostEntity post = await _uow.PostRepository.Get(Id);
            PostMetricEntity metric = await _uow.PostMetricRepository.Get(post);

            return Ok(new Response(
                "success",
                "Post metric founded!!",
                200,
                metric
            ));
        }

        

    }
}