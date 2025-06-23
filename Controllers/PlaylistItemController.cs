using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
            PlaylistEntity play = await _uow.PlaylistRepository.Get(dto.Playlist);
            PostEntity post = await _uow.PostRepository.Get(dto.PostId);
            PlaylistItemEntity result = await _uow.PlaylistItemRepository.AddPostToPlaylist(play, post, dto.Order);

            await _uow.PlaylistRepository.SumOrReduceItemCount(play, Blog.utils.enums.SumOrRedEnum.SUM);

            return Ok(new Response(
                "success",
                "Post added in play list: " + play.Name,
                201,
                result
            ));
        }

        [HttpDelete("itemId:required")]
        [EnableRateLimiting("DeleteItemPolicy")]
        public async Task<IActionResult> RemovePostFromPlaylist(ulong itemId)
        {
            PlaylistItemEntity item = await _uow.PlaylistItemRepository.Get(itemId);
            await _uow.PlaylistItemRepository.RemovePostFromPlaylist(item);

            PlaylistEntity play = await _uow.PlaylistRepository.Get(item.PlaylistId);
            await _uow.PlaylistRepository.SumOrReduceItemCount(play, Blog.utils.enums.SumOrRedEnum.REDUCE);

            return Ok(new Response(
                "success",
                "Post removed!!!",
                201,
                null
            ));
        }

        [HttpGet("{playId:required}")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfPlaylistPaginated(ulong playId , [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            PlaylistEntity play = await _uow.PlaylistRepository.Get(playId);
            PaginatedList<PlaylistItemEntity> result = await _uow.PlaylistItemRepository.GetAllOfPlaylistPaginated(play, pageNumber, pageSize);
            result.Code = 200;

            return Ok(result);
        }

        [HttpPut]
        [EnableRateLimiting("UpdateItemPolicy")]
        public async Task<IActionResult> UpdateOrder([FromBody] UpdatePlaylistItemDTO dto)
        {
            PlaylistItemEntity item = await _uow.PlaylistItemRepository.Get(dto.PlaylistItem);
            PlaylistItemEntity update = await _uow.PlaylistItemRepository.UpdateOrder(item, dto.Order);

            return Ok(new Response(
                "success",
                "Play list item updated!!!",
                200,
                update
            ));
        }

    }
}