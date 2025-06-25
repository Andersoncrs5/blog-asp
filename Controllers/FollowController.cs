using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using blog.entities;
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

            ApplicationUser follower = await _uow.UserRepository.Get(followerId);
            ApplicationUser followed = await _uow.UserRepository.Get(followedUserId);

            FollowEntity result = await _uow.FollowRepository.FollowAsync(follower, followed);

            return Ok(new Response(
                "success",
                $"You are now following user: {followed.UserName ?? followed.Id}",
                200,
                result
            ));
        }

        [HttpDelete("{followedUserId:required}")]
        [EnableRateLimiting("FollowOrUnfollowPolicy")]
        public async Task<IActionResult> UnfollowAsync(string followedUserId)
        {
            string? followerId = User.FindFirst(ClaimTypes.Sid)?.Value;
            ApplicationUser follower = await _uow.UserRepository.Get(followerId);
            ApplicationUser followed = await _uow.UserRepository.Get(followedUserId);

            await _uow.FollowRepository.UnfollowAsync(follower, followed);

            return NoContent(); 
        }

        [HttpGet("get-my-followers")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetMyFollowersAsync([FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            ApplicationUser user = await _uow.UserRepository.Get(userId);

            PaginatedList<FollowEntity> result = await _uow.FollowRepository.GetFollowersAsync(user, pageNumber, pageSize);

            result.Code = 200;
            return Ok(result);
        }

        [HttpGet("{userId:required}/followers")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetFollowersForUserAsync(string userId, [FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        {
            ApplicationUser user = await _uow.UserRepository.Get(userId);

            PaginatedList<FollowEntity> result = await _uow.FollowRepository.GetFollowersAsync(user, pageNumber, pageSize);

            result.Code = 200;
            return Ok(result);
        }

        [HttpGet("get-my-following")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetMyFollowingAsync([FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            ApplicationUser user = await _uow.UserRepository.Get(userId);

            PaginatedList<FollowEntity> result = await _uow.FollowRepository.GetFollowingAsync(user, pageNumber, pageSize);

            result.Code = 200;
            return Ok(result);
        }

        [HttpGet("{userId:required}/following")] 
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetFollowingForUserAsync(string userId, [FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        {
            ApplicationUser? user = await _uow.UserRepository.Get(userId);
            PaginatedList<FollowEntity> result = await _uow.FollowRepository.GetFollowingAsync(user, pageNumber, pageSize);

            result.Code = 200;
            return Ok(result);
        }

        [HttpPut("toggle-notifications/{followedUserId:required}")] 
        [EnableRateLimiting("fixedWindowLimiterPolicy")]
        public async Task<IActionResult> ToggleNotificationStatus(string followedUserId)
        {
            
            string? followerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            FollowEntity result = await _uow.FollowRepository.ChangeStatusReceiveNotifications(followerId, followedUserId);

            string message = result.ReceiveNotifications ?
                $"Notifications enabled for user: {followedUserId}" :
                $"Notifications disabled for user: {followedUserId}";

            return Ok(new Response(
                "success",
                message,
                200,
                result
            ));
        }

    }
}
