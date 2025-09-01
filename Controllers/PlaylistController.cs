using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using blog.utils.Filters.FiltersDTO;
using blog.utils.Filters.FiltersQuerys;
using blog.utils.Responses;
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
        public async Task<IActionResult> Create([FromBody] CreatePlaylistDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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

            PlaylistEntity result = await _uow.PlaylistRepository.Create(user, dto);

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

            await _uow.UserMetricRepository.SumOrRedPlaylistCount(metric, Blog.utils.enums.SumOrRedEnum.SUM);

            return StatusCode(201, new ResponseBody<PlaylistEntity>
            {
                Body = result,
                Code = 201,
                Message = "Play list created!!",
                Status = true,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("{Id:required}")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<ActionResult<PlaylistEntity?>> Get(ulong Id)
        {
            PlaylistEntity? play = await _uow.PlaylistRepository.Get(Id);
            if (play == null)
            {
                return StatusCode(404, new ResponseBody<PlaylistEntity>
                {
                    Body = null,
                    Code = 404,
                    Message = "Play list not found!!",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            return StatusCode(200, new ResponseBody<PlaylistEntity>
            {
                Body = play,
                Code = 200,
                Message = "Play list found!!",
                Status = true,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpPut("{Id:required}")]
        [EnableRateLimiting("UpdateItemPolicy")]
        public async Task<ActionResult<PlaylistEntity?>> Update(ulong Id, [FromBody] UpdatePlaylistDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            PlaylistEntity? play = await _uow.PlaylistRepository.Get(Id);
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

            PlaylistEntity result = await _uow.PlaylistRepository.Update(play, dto);

            return StatusCode(200, new ResponseBody<PlaylistEntity>
            {
                Body = result,
                Code = 200,
                Message = "Play list updated!!",
                Status = true,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpDelete("{Id:required}")]
        [EnableRateLimiting("DeleteItemPolicy")]
        public async Task<ActionResult<PlaylistEntity?>> Delete(ulong Id)
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ResponseBody<PlaylistEntity>
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
                return NotFound(new ResponseBody<PlaylistEntity>
                {
                    Body = null,
                    Code = 404,
                    Message = "User not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            PlaylistEntity? play = await _uow.PlaylistRepository.Get(Id);
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

            await _uow.PlaylistRepository.Delete(play);

            UserMetricEntity? metric = await _uow.UserMetricRepository.Get(user.Id);
            if (metric == null)
            {
                return NotFound(new ResponseBody<PlaylistEntity>
                {
                    Body = null,
                    Code = 404,
                    Datetime = DateTimeOffset.Now,
                    Message = "User metric not found",
                    Status = false
                });
            }

            await _uow.UserMetricRepository.SumOrRedPlaylistCount(metric, Blog.utils.enums.SumOrRedEnum.REDUCE);

            return StatusCode(200, new ResponseBody<PlaylistEntity>
            {
                Body = null,
                Code = 200,
                Message = "Play list deleted!!",
                Status = true,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpPatch("{Id:required}/change-status-public")]
        [EnableRateLimiting("UpdateItemPolicy")]
        public async Task<IActionResult> ChangeStatusIsPublic(ulong Id)
        {
            PlaylistEntity? play = await _uow.PlaylistRepository.Get(Id);
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

            PlaylistEntity result = await _uow.PlaylistRepository.ChangeStatusIsPublic(play);

            return StatusCode(200, new ResponseBody<PlaylistEntity>
            {
                Body = result,
                Code = 200,
                Message = "Status play list changed!!",
                Status = true,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("get-all-user-paginated")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfUserPaginated([FromQuery] PlaylistFilterDTO filter)
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

            IQueryable<PlaylistEntity> query = _uow.PlaylistRepository.GetAllOfUserQuery(user);

            query = PlaylistQueryFilter.ApplyFilters(query, filter);

            PaginatedList<PlaylistEntity> result = await PaginatedList<PlaylistEntity>.CreateAsync(query, filter.PageNumber, filter.PageSize);

            return Ok(new ResponseBody<PaginatedList<PlaylistEntity>>
            {
                Status = true,
                Message = "All Play list",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("{userId:required}/get-all-user-paginated")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfUserPaginated(string userId, [FromQuery] PlaylistFilterDTO filter)
        {
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

            IQueryable<PlaylistEntity> query = _uow.PlaylistRepository.GetAllOfUserQuery(user);

            query = PlaylistQueryFilter.ApplyFilters(query, filter);

            query = query.Where(c => c.IsPublic == true);

            PaginatedList<PlaylistEntity> result = await PaginatedList<PlaylistEntity>.CreateAsync(query, filter.PageNumber, filter.PageSize);

            return Ok(new ResponseBody<PaginatedList<PlaylistEntity>>
            {
                Status = true,
                Message = "All Play list",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("get-all-user-paginated/{userId:required}")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfUserPaginated(string userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
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

            PaginatedList<PlaylistEntity> result = await _uow.PlaylistRepository.GetAllOfUserPaginated(user, pageNumber, pageSize, true);

            return Ok(new ResponseBody<PaginatedList<PlaylistEntity>>
            {
                Status = true,
                Message = "All Play list",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }


    }
}