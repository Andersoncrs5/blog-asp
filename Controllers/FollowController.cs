using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using blog.entities;
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
    public class FollowController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public FollowController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpPost("{followedUserId:required}")]
        [EnableRateLimiting("FollowOrUnfollowPolicy")]
        public async Task<IActionResult> FollowAsync(string followedUserId)
        {
            string? followerId = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (string.IsNullOrWhiteSpace(followerId))
            {
                return Unauthorized(new ResponseBody<ApplicationUser>
                {
                    Body = null,
                    Code = 401,
                    Message = "You are not authorizetion",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            if (followerId == followedUserId)
            {
                return StatusCode(StatusCodes.Status409Conflict, new ResponseBody<FollowEntity>
                {
                    Status = true,
                    Message = $"You cannot following yourself",
                    Code = 200,
                    Body = null,
                    Datetime = DateTimeOffset.Now
                });
            }

            bool check = await _uow.FollowRepository.Exists(followerId, followedUserId);
            if (check)
            {
                return Conflict(new ResponseBody<FollowEntity>
                {
                    Status = false,
                    Message = $"You are already following user",
                    Code = 409,
                    Body = null,
                    Datetime = DateTimeOffset.Now
                });
            }

            ApplicationUser? follower = await _uow.UserRepository.Get(followerId);
            if (follower == null)
            {
                return NotFound(new ResponseBody<ApplicationUser>
                {
                    Body = null,
                    Code = 404,
                    Message = "follower not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            ApplicationUser? followed = await _uow.UserRepository.Get(followedUserId);
            if (followed == null)
            {
                return NotFound(new ResponseBody<ApplicationUser>
                {
                    Body = null,
                    Code = 404,
                    Message = "followed not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            FollowEntity result = await _uow.FollowRepository.FollowAsync(follower, followed);

            return Ok(new ResponseBody<FollowEntity>
            {
                Status = true,
                Message = $"You are now following user: {followed.UserName ?? followed.Id}",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpDelete("{followedUserId:required}")]
        [EnableRateLimiting("FollowOrUnfollowPolicy")]
        public async Task<IActionResult> UnfollowAsync(string followedUserId)
        {
            string? followerId = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (string.IsNullOrWhiteSpace(followerId))
            {
                return Unauthorized(new ResponseBody<ApplicationUser>
                {
                    Body = null,
                    Code = 401,
                    Message = "You are not authorizetion",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            ApplicationUser? follower = await _uow.UserRepository.Get(followerId);
            if (follower == null)
            {
                return NotFound(new ResponseBody<ApplicationUser>
                {
                    Body = null,
                    Code = 404,
                    Message = "follower not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            ApplicationUser? followed = await _uow.UserRepository.Get(followedUserId);
            if (followed == null)
            {
                return NotFound(new ResponseBody<ApplicationUser>
                {
                    Body = null,
                    Code = 404,
                    Message = "followed not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            FollowEntity? follow = await _uow.FollowRepository.GetAsync(followerId, followedUserId);
            if (follow == null)
            {
                return NotFound(new ResponseBody<ApplicationUser>
                {
                    Body = null,
                    Code = 404,
                    Message = $"You are not following user: {followed.UserName}.",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            await _uow.FollowRepository.UnfollowAsync(follow);

            return Ok(new ResponseBody<string>
            {
                Status = true,
                Message = $"You are now not following user: {followed.UserName ?? followed.Id}",
                Code = 200,
                Body = null,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("get-my-followers")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetMyFollowersAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ResponseBody<ApplicationUser>
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
                return NotFound(new ResponseBody<ApplicationUser>
                {
                    Body = null,
                    Code = 404,
                    Message = "User not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            PaginatedList<FollowEntity> result = await _uow.FollowRepository.GetFollowersAsync(user, pageNumber, pageSize);

            return Ok(new ResponseBody<PaginatedList<FollowEntity>>
            {
                Status = true,
                Message = "All follower",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("{userId:required}/followers")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetFollowersForUserAsync(string userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ResponseBody<ApplicationUser>
                {
                    Body = null,
                    Code = 401,
                    Message = "User id is required",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            ApplicationUser? user = await _uow.UserRepository.Get(userId);
            if (user == null)
            {
                return NotFound(new ResponseBody<ApplicationUser>
                {
                    Body = null,
                    Code = 404,
                    Message = "User not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            PaginatedList<FollowEntity> result = await _uow.FollowRepository.GetFollowersAsync(user, pageNumber, pageSize);

            return Ok(new ResponseBody<PaginatedList<FollowEntity>>
            {
                Status = true,
                Message = "All follower",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("get-my-following")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetMyFollowingAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ResponseBody<ApplicationUser>
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
                    Datetime = DateTimeOffset.Now,
                    Message = "User metric not found",
                    Status = false
                });
            }

            PaginatedList<FollowEntity> result = await _uow.FollowRepository.GetFollowingAsync(user, pageNumber, pageSize);

            return Ok(new ResponseBody<PaginatedList<FollowEntity>>
            {
                Status = true,
                Message = "All follower",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("{userId:required}/following")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetFollowingForUserAsync(string userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ResponseBody<ApplicationUser>
                {
                    Body = null,
                    Code = 401,
                    Message = "User id is required",
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
                    Datetime = DateTimeOffset.Now,
                    Message = "User not found",
                    Status = false
                });
            }

            PaginatedList<FollowEntity> result = await _uow.FollowRepository.GetFollowingAsync(user, pageNumber, pageSize);

            return Ok(new ResponseBody<PaginatedList<FollowEntity>>
            {
                Status = true,
                Message = "All follower",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpPut("toggle-notifications/{followedUserId:required}")]
        [EnableRateLimiting("fixedWindowLimiterPolicy")]
        public async Task<IActionResult> ToggleNotificationStatus(string followedUserId)
        {
            string? followerId = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (string.IsNullOrWhiteSpace(followerId))
            {
                return Unauthorized(new ResponseBody<ApplicationUser>
                {
                    Body = null,
                    Code = 401,
                    Message = "You are not authorizetion",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            FollowEntity? follow = await _uow.FollowRepository.GetAsync(followerId, followedUserId);
            if (follow == null)
            {
                return NotFound(new ResponseBody<ApplicationUser>
                {
                    Body = null,
                    Code = 404,
                    Message = $"You are not following user",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            FollowEntity result = await _uow.FollowRepository.ChangeStatusReceiveNotifications(follow);

            string message = result.ReceiveNotifications ?
                $"Notifications enabled for user: {followedUserId}" :
                $"Notifications disabled for user: {followedUserId}";

            return Ok(new ResponseBody<FollowEntity>
            {
                Status = true,
                Message = message,
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

    }
}
