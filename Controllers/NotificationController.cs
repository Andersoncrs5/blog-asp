using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using blog.DTOs.Notification;
using blog.entities;
using blog.utils.Filters.FiltersDTO;
using blog.utils.Filters.FiltersQuerys;
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
    public class NotificationController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public NotificationController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet("{id:required:long}")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> Get(long id)
        {
            string? currentUserId = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Unauthorized(new ResponseBody<string>
                {
                    Body = null,
                    Code = 401,
                    Message = "Id is required",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            NotificationEntity? noti = await _uow.NotificationRepository.GetAsync(id);
            if (noti == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Message = "Notification not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            if (noti.ApplicationUserId != currentUserId)
            {
                return StatusCode(StatusCodes.Status409Conflict, new ResponseBody<string>
                {
                    Status = true,
                    Message = "You do not have permission to access this notification.",
                    Code = 409,
                    Body = null,
                    Datetime = DateTimeOffset.Now
                });
            }

            return StatusCode(StatusCodes.Status200OK, new ResponseBody<NotificationEntity>
            {
                Status = true,
                Message = "Notification founded!",
                Code = 200,
                Body = noti,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpDelete("{id:required:long}")]
        [EnableRateLimiting("DeleteItemPolicy")]
        public async Task<IActionResult> Remove(long id)
        {
            string? currentUserId = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Unauthorized(new ResponseBody<string>
                {
                    Body = null,
                    Code = 401,
                    Message = "Id is required",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            NotificationEntity? noti = await _uow.NotificationRepository.GetAsync(id);
            if (noti == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Message = "Notification not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            if (noti.ApplicationUserId != currentUserId)
            {
                return StatusCode(StatusCodes.Status409Conflict, new ResponseBody<string>
                {
                    Status = true,
                    Message = "You do not have permission to delete this notification.",
                    Code = 409,
                    Body = null,
                    Datetime = DateTimeOffset.Now
                });
            }

            await _uow.NotificationRepository.RemoveAsync(noti);

            return NoContent();
        }

        [HttpPost("send/{recipientUserId:required}")]
        [EnableRateLimiting("CreateItemPolicy")]
        [Authorize(Roles = "AdminRole, SuperAdminRole")]
        public async Task<IActionResult> SendNotification([FromBody] CreateNotificationDTO dto, string recipientUserId)
        {
            string? senderUserId = User.FindFirst(ClaimTypes.Sid)?.Value;

            if (string.IsNullOrWhiteSpace(senderUserId))
            {
                return Unauthorized(new ResponseBody<string>
                {
                    Body = null,
                    Code = 401,
                    Message = "Id is required",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            ApplicationUser? recipientUser = await _uow.UserRepository.Get(recipientUserId);
            if (recipientUser == null)
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

            NotificationEntity newNotification = await _uow.NotificationRepository.SendNotification(dto, recipientUser, senderUserId);

            return StatusCode(200, new ResponseBody<NotificationEntity>
            {
                Status = true,
                Message = "Notification sent successfully!",
                Code = 200,
                Body = newNotification,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpPost("send-new-post-notification/{postId:required:long}")]
        [EnableRateLimiting("CreateItemPolicy")]
        public async Task<IActionResult> SendNewPostNotificationToFollowers(long postId)
        {
            string? postAuthorId = User.FindFirst(ClaimTypes.Sid)?.Value;

            if (string.IsNullOrWhiteSpace(postAuthorId))
            {
                return Unauthorized(new ResponseBody<string>
                {
                    Body = null,
                    Code = 401,
                    Message = "Id is required",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            ApplicationUser? postAuthor = await _uow.UserRepository.Get(postAuthorId);
            if (postAuthor == null)
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

            PostEntity? newPost = await _uow.PostRepository.Get(postId);
            if (newPost == null)
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

            if (newPost.ApplicationUserId != postAuthorId)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ResponseBody<NotificationEntity>
                {
                    Status = true,
                    Message = "You do not have permission to send notifications for this post.",
                    Code = 403,
                    Body = null,
                    Datetime = DateTimeOffset.Now
                });
            }

            await _uow.NotificationRepository.SendNotificationToFollowersAboutNewPost(postAuthor, newPost);

            return Ok(new ResponseBody<string>
            {
                Status = true,
                Message = "Notifications for new post sent to followers.",
                Code = 200,
                Body = null,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("my-notifications")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetMyNotifications([FromQuery] NotificationFilter filter)
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ResponseBody<string>
                {
                    Body = null,
                    Code = 401,
                    Message = "Id is required",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            IQueryable<NotificationEntity> query = _uow.NotificationRepository.GetUserNotifications(userId);

            IQueryable<NotificationEntity> queryWithFilter = NotificationQueryFilter.ApplyFilters(query, filter);

            PaginatedList<NotificationEntity> result = await PaginatedList<NotificationEntity>.CreateAsync(queryWithFilter, filter.PageNumber, filter.PageSize);

            return Ok(new ResponseBody<PaginatedList<NotificationEntity>>
            {
                Status = true,
                Message = "All Notifications",
                Code = 200,
                Body = result,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpPut("mark-as-read")]
        [EnableRateLimiting("UpdateItemPolicy")]
        public async Task<IActionResult> MarkNotificationsAsRead([FromBody] List<long> notificationIds)
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

            if (notificationIds == null || !notificationIds.Any())
            {
                return BadRequest(new ResponseBody<string>
                {
                    Status = false,
                    Message = "No notification IDs provided.",
                    Code = 400,
                    Body = null,
                    Datetime = DateTimeOffset.Now
                });
            }

            int updatedCount = await _uow.NotificationRepository.MarkNotificationsAsReadAsync(notificationIds, userId);

            return Ok(new ResponseBody<string>
            {
                Status = true,
                Message = $"{updatedCount} notifications marked as read.",
                Code = 200,
                Body = null,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("unread-count")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetUnreadNotificationsCount()
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

            int unreadCount = await _uow.NotificationRepository.GetUnreadNotificationsCountAsync(userId);

            return Ok(new ResponseBody<string>
            {
                Status = true,
                Message = "Unread notifications count fetched successfully.",
                Code = 200,
                Body = null,
                Datetime = DateTimeOffset.Now
            });
        }
    }
}