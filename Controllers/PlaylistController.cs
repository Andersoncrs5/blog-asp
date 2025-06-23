using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Blog.DTOs.Playlist;
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
    public class PlaylistController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public PlaylistController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpPost]
        [EnableRateLimiting("CreateItemPolicy")]
        public async Task<IActionResult> Create([FromBody] CreatePlaylistDTO dto )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            ApplicationUser user = await _uow.UserRepository.Get(userId);

            PlaylistEntity result = await _uow.PlaylistRepository.Create(user, dto);

            UserMetricEntity metric = await _uow.UserMetricRepository.Get(user.Id);
            await _uow.UserMetricRepository.SumOrRedPlaylistCount(metric, Blog.utils.enums.SumOrRedEnum.SUM);

            return Ok(new Response(
                "success",
                "Play list created!!",
                201,
                result
            ));
        }

        [HttpGet("{Id:required}")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> Get(ulong Id)
        {
            PlaylistEntity play = await _uow.PlaylistRepository.Get(Id);

            return Ok(new Response(
                "success",
                "Play list founded!!",
                200,
                play
            ));
        }

        [HttpPut("{Id:required}")]
        [EnableRateLimiting("UpdateItemPolicy")]
        public async Task<IActionResult> Update(ulong Id, [FromBody] UpdatePlaylistDTO dto )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
                
            PlaylistEntity play = await _uow.PlaylistRepository.Get(Id);
            PlaylistEntity result = await _uow.PlaylistRepository.Update(play, dto);

            return Ok(new Response(
                "success",
                "Play list updated!!",
                200,
                result
            ));
        }

        [HttpDelete("{Id:required}")]
        [EnableRateLimiting("DeleteItemPolicy")]
        public async Task<IActionResult> Delete(ulong Id)
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            ApplicationUser user = await _uow.UserRepository.Get(userId);

            PlaylistEntity play = await _uow.PlaylistRepository.Get(Id);
            await _uow.PlaylistRepository.Delete(play);

            UserMetricEntity metric = await _uow.UserMetricRepository.Get(user.Id);
            await _uow.UserMetricRepository.SumOrRedPlaylistCount(metric, Blog.utils.enums.SumOrRedEnum.REDUCE);

            return Ok(new Response(
                "success",
                "Play list deleted!!",
                200,
                play
            ));
        }

        [HttpPatch("{Id:required}/change-status-public")]
        [EnableRateLimiting("UpdateItemPolicy")]
        public async Task<IActionResult> ChangeStatusIsPublic(ulong Id)
        {
            PlaylistEntity play = await _uow.PlaylistRepository.Get(Id);
            PlaylistEntity result = await _uow.PlaylistRepository.ChangeStatusIsPublic(play);

            return Ok(new Response(
                "success",
                "Status play list changed!!",
                200,
                result
            ));
        }

        [HttpGet("get-all-user-paginated")] 
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfUserPaginated([FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value; 
            ApplicationUser user = await _uow.UserRepository.Get(userId);

            PaginatedList<PlaylistEntity> result = await _uow.PlaylistRepository.GetAllOfUserPaginated(user, pageNumber, pageSize);
            
            result.Code = 200;
            return Ok(result);
        }

        [HttpGet("get-all-user-paginated/{userId:required}")] 
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfUserPaginated(string userId, [FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        {
            ApplicationUser user = await _uow.UserRepository.Get(userId);

            PaginatedList<PlaylistEntity> result = await _uow.PlaylistRepository.GetAllOfUserPaginated(user, pageNumber, pageSize, true);
            
            result.Code = 200;
            return Ok(result);
        }


    }
}