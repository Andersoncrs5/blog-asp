using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            MediaPostEntity media = await _uow.MediaPostRepository.GetAsync(Id);

            return Ok(new Response(
                "success",
                "Media founded with successfully",
                200,
                media
            ));
        }

        [HttpDelete("{Id:required}")]
        [EnableRateLimiting("DeleteItemPolicy")]
        public async Task<IActionResult> Delete(ulong Id)
        {
            MediaPostEntity media = await _uow.MediaPostRepository.GetAsync(Id);
            await _uow.MediaPostRepository.DeleteAsync(media);

            PostEntity post = await _uow.PostRepository.Get(media.PostId);
            PostMetricEntity postMetric = await _uow.PostMetricRepository.Get(post);
            await _uow.PostMetricRepository.SumOrRedMediaCount(postMetric, Blog.utils.enums.SumOrRedEnum.REDUCE);

            return Ok(new Response(
                "success",
                "Media deleted",
                200,
                media
            ));
        }

        [HttpGet("{postId:required}/get-all-post")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfPost(long postId,[FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        {
            PostEntity post = await _uow.PostRepository.Get(postId);
            PaginatedList<MediaPostEntity> result = await _uow.MediaPostRepository.GetAllOfPostPaginatedListAsync(post, pageNumber, pageSize);
            result.Code = 200;
            
            return Ok(result);
        }

        [HttpPost("{postId:required}")]
        [EnableRateLimiting("CreateItemPolicy")]
        public async Task<IActionResult> Create(long postId, [FromBody] CreateMediaDTO dto )
        {
            PostEntity post = await _uow.PostRepository.Get(postId);
            MediaPostEntity media = await _uow.MediaPostRepository.CreateAsync(post, dto);

            PostMetricEntity postMetric = await _uow.PostMetricRepository.Get(post);
            await _uow.PostMetricRepository.SumOrRedMediaCount(postMetric, Blog.utils.enums.SumOrRedEnum.SUM);

            return Ok(new Response(
                "success",
                "Media created with successfully",
                200,
                media
            ));
        }

        [HttpPut("{Id:required}")]
        [EnableRateLimiting("UpdateItemPolicy")]
        public async Task<IActionResult> Update(ulong Id, [FromBody] UpdateMediaDTO dto)
        {
            MediaPostEntity media = await _uow.MediaPostRepository.GetAsync(Id);
            MediaPostEntity result = await _uow.MediaPostRepository.UpdateAsync(media, dto);

            return Ok(new Response(
                "success",
                "Media updated with successfully",
                200,
                media
            ));
        }

    }
}