using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Blog.DTOs.PlaylistItem;
using Blog.entities;
using Blog.SetUnitOfWork;
using Blog.utils;
using Microsoft.AspNetCore.Mvc;

namespace blog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlaylistItemController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public PlaylistItemController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpPost]
        public async Task<IActionResult> AddPostToPlaylist([FromBody] CreatePlaylistItemDTO dto)
        {
            PlaylistEntity play = await _uow.PlaylistRepository.Get(dto.Playlist);
            PostEntity post = await _uow.PostRepository.Get(dto.PostId);
            PlaylistItemEntity result = await _uow.PlaylistItemRepository.AddPostToPlaylist(play, post, dto.Order);

            return Ok(new Response(
                "success",
                "Post added in play list: " + play.Name,
                201,
                result
            ));
        }

        [HttpDelete("itemId:required")]
        public async Task<IActionResult> RemovePostFromPlaylist(ulong itemId)
        {
            PlaylistItemEntity item = await _uow.PlaylistItemRepository.Get(itemId);
            await _uow.PlaylistItemRepository.RemovePostFromPlaylist(item);

            return Ok(new Response(
                "success",
                "Post removed!!!",
                201,
                null
            ));
        }

        [HttpGet("{playId:required}")]
        public async Task<IActionResult> GetAllOfPlaylistPaginated(ulong playId , [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            PlaylistEntity play = await _uow.PlaylistRepository.Get(playId);
            PaginatedList<PlaylistItemEntity> result = await _uow.PlaylistItemRepository.GetAllOfPlaylistPaginated(play, pageNumber, pageSize);
            result.Code = 200;

            return Ok(result);
        }

        [HttpPut]
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