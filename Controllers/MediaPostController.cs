using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blog.utils.Filters.FiltersDTO;
using blog.utils.Filters.FiltersQuerys;
using blog.utils.Responses;
using Blog.DTOs.Media;
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
    public class MediaPostController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public MediaPostController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet("{Id:required}")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAsync(ulong Id)
        {
            MediaPostEntity? media = await _uow.MediaPostRepository.GetAsync(Id);
            if (media == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Datetime = DateTimeOffset.Now,
                    Message = "Media not found",
                    Status = false
                });
            }

            return Ok(new ResponseBody<MediaPostEntity>
            {
                Status = true,
                Message = "Media founded with successfully",
                Code = 200,
                Body = media,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpDelete("{Id:required}")]
        [EnableRateLimiting("DeleteItemPolicy")]
        public async Task<IActionResult> Delete(ulong Id)
        {
            MediaPostEntity? media = await _uow.MediaPostRepository.GetAsync(Id);
            if (media == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Datetime = DateTimeOffset.Now,
                    Message = "Media not found",
                    Status = false
                });
            }

            await _uow.MediaPostRepository.DeleteAsync(media);

            PostEntity? post = await _uow.PostRepository.Get(media.PostId);
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
            
            PostMetricEntity metricUpdate = await _uow.PostMetricRepository.SumOrRedMediaCount(postMetric, Blog.utils.enums.SumOrRedEnum.REDUCE);

            await _uow.PostRepository.CalculateEngagementScore(post, metricUpdate);

            return Ok(new ResponseBody<string>
            {
                Status = true,
                Message = "Media deleted with successfully",
                Code = 200,
                Body = null,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("{postId:required}/get-all-post")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfPost(long postId, [FromQuery] MediaPostFilter filter)
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

            IQueryable<MediaPostEntity> query = _uow.MediaPostRepository.GetAllOfPost(post);

            IQueryable<MediaPostEntity> queryWitFilters = MediaPostQueryFilter.ApplyFilters(query, filter);

            PaginatedList<MediaPostEntity> result = await PaginatedList<MediaPostEntity>.CreateAsync(queryWitFilters, filter.PageNumber, filter.PageSize);
            
            return Ok(new ResponseBody<PaginatedList<MediaPostEntity>>
            {
                Status = true,
                Message = "All Medias",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpPost("{postId:required}")]
        [EnableRateLimiting("CreateItemPolicy")]
        public async Task<IActionResult> Create(long postId, [FromBody] CreateMediaDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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

            int amount = await _uow.MediaPostRepository.CheckAmountMediaByPost(postId);      
            if (amount > 10) 
            {
                return BadRequest(new ResponseBody<string>
                {
                    Body = null,
                    Code = 400,
                    Message = "Media limit by post is 10",
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
            
            MediaPostEntity media = await _uow.MediaPostRepository.CreateAsync(post, dto);

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

            PostMetricEntity metricUpdate = await _uow.PostMetricRepository.SumOrRedMediaCount(postMetric, Blog.utils.enums.SumOrRedEnum.SUM);

            await _uow.PostRepository.CalculateEngagementScore(post, metricUpdate);

            return StatusCode(201, new ResponseBody<MediaPostEntity>
            {
                Status = true,
                Message = "Media created with successfully",
                Code = 201,
                Body = media,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpPut("{Id:required}")]
        [EnableRateLimiting("UpdateItemPolicy")]
        public async Task<IActionResult> Update(ulong Id, [FromBody] UpdateMediaDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            MediaPostEntity? media = await _uow.MediaPostRepository.GetAsync(Id);
            if (media == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Datetime = DateTimeOffset.Now,
                    Message = "Media not found",
                    Status = false
                });
            }

            MediaPostEntity result = await _uow.MediaPostRepository.UpdateAsync(media, dto);

            return StatusCode(200, new ResponseBody<MediaPostEntity>
            {
                Status = true,
                Message = "Media updated with successfully",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

    }
}