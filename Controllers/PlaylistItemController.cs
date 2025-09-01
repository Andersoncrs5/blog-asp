using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using blog.utils.Responses;
using Blog.DTOs.PlaylistItem;
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
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PlaylistItemController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public PlaylistItemController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpPost]
        [EnableRateLimiting("CreateItemPolicy")]
        public async Task<IActionResult> AddPostToPlaylist([FromBody] CreatePlaylistItemDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bool check = await _uow.PlaylistItemRepository.Exists(dto.Playlist, dto.PostId);
            if (check)
            {
                return Conflict(new ResponseBody<string>
                {
                    Body = null,
                    Code = 409,
                    Message = "This post is already in the playlist",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            PlaylistEntity? play = await _uow.PlaylistRepository.Get(dto.Playlist);
            if (play == null)
            {
                return StatusCode(404, new ResponseBody<PlaylistEntity>
                {
                    Body = null,
                    Code = 404,
                    Message = "Play list not found!!",
                    Status = true,
                    Datetime = DateTimeOffset.Now
                });
            }

            PostEntity? post = await _uow.PostRepository.Get(dto.PostId);
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

            PlaylistItemEntity result = await _uow.PlaylistItemRepository.AddPostToPlaylist(play, post, dto.Order);

            await _uow.PlaylistRepository.SumOrReduceItemCount(play, Blog.utils.enums.SumOrRedEnum.SUM);

            return StatusCode(201, new ResponseBody<PlaylistItemEntity>
            {
                Body = result,
                Code = 201,
                Message = "Post added in play list: " + play.Name,
                Status = true,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpDelete("itemId:required")]
        [EnableRateLimiting("DeleteItemPolicy")]
        public async Task<IActionResult> RemovePostFromPlaylist(ulong itemId)
        {
            PlaylistItemEntity? item = await _uow.PlaylistItemRepository.Get(itemId);
            if (item == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Message = "Playlist Item not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            await _uow.PlaylistItemRepository.RemovePostFromPlaylist(item);

            PlaylistEntity? play = await _uow.PlaylistRepository.Get(item.PlaylistId);
            if (play == null)
            {
                return StatusCode(404, new ResponseBody<PlaylistEntity>
                {
                    Body = null,
                    Code = 404,
                    Message = "Play list not found!!",
                    Status = true,
                    Datetime = DateTimeOffset.Now
                });
            }

            await _uow.PlaylistRepository.SumOrReduceItemCount(play, Blog.utils.enums.SumOrRedEnum.REDUCE);

            return StatusCode(200, new ResponseBody<PlaylistItemEntity>
            {
                Body = null,
                Code = 200,
                Message = "Post removed!!!",
                Status = true,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("{playId:required}")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfPlaylistPaginated(ulong playId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            PlaylistEntity? play = await _uow.PlaylistRepository.Get(playId);
            if (play == null)
            {
                return StatusCode(404, new ResponseBody<PlaylistEntity>
                {
                    Body = null,
                    Code = 404,
                    Message = "Play list not found!!",
                    Status = true,
                    Datetime = DateTimeOffset.Now
                });
            }

            PaginatedList<PlaylistItemEntity> result = await _uow.PlaylistItemRepository.GetAllOfPlaylistPaginated(play, pageNumber, pageSize);

            return Ok(new ResponseBody<PaginatedList<PlaylistItemEntity>>
            {
                Status = true,
                Message = "All Play list item",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpPut]
        [EnableRateLimiting("UpdateItemPolicy")]
        public async Task<IActionResult> UpdateOrder([FromBody] UpdatePlaylistItemDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            PlaylistItemEntity? item = await _uow.PlaylistItemRepository.Get(dto.PlaylistItem);
            if (item == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Message = "Playlist Item not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            PlaylistItemEntity update = await _uow.PlaylistItemRepository.UpdateOrder(item, dto.Order);

            return StatusCode(200, new ResponseBody<PlaylistItemEntity>
            {
                Body = null,
                Code = 200,
                Message = "Play list item updated!!!",
                Status = true,
                Datetime = DateTimeOffset.Now
            });
        }

    }
}