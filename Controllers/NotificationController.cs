using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using blog.DTOs.Notification;
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

            NotificationEntity noti = await _uow.NotificationRepository.GetAsync(id);

            if (noti.ApplicationUserId != currentUserId)
                return BadRequest(new Response("fail", "You do not have permission to access this notification.", 403, null));

            return Ok(new Response(
                "success",
                "Notification founded!",
                200,
                noti
            ));
        }

        [HttpDelete("{id:required:long}")]
        [EnableRateLimiting("DeleteItemPolicy")]
        public async Task<IActionResult> Remove(long id)
        {
            
            string? currentUserId = User.FindFirst(ClaimTypes.Sid)?.Value;

            NotificationEntity noti = await _uow.NotificationRepository.GetAsync(id);

            if (noti.ApplicationUserId != currentUserId)
            {
                return BadRequest(new Response("fail", "You do not have permission to delete this notification.", 403, null));
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

            ApplicationUser recipientUser = await _uow.UserRepository.Get(recipientUserId);

            NotificationEntity newNotification = await _uow.NotificationRepository.SendNotification(dto, recipientUser, senderUserId);

            return StatusCode(201, new Response(
                "success",
                "Notification sent successfully!",
                201,
                newNotification
            ));
        }

        [HttpPost("send-new-post-notification/{postId:required:long}")]
        [EnableRateLimiting("CreateItemPolicy")]
        public async Task<IActionResult> SendNewPostNotificationToFollowers(long postId)
        {
            string? postAuthorId = User.FindFirst(ClaimTypes.Sid)?.Value;

            ApplicationUser postAuthor = await _uow.UserRepository.Get(postAuthorId);

            PostEntity newPost = await _uow.PostRepository.Get(postId);

            if (newPost.ApplicationUserId != postAuthorId)
                return BadRequest(new Response("fail", "You do not have permission to send notifications for this post.", 403, null));
            

            await _uow.NotificationRepository.SendNotificationToFollowersAboutNewPost(postAuthor, newPost);

            return Ok(new Response(
                "success",
                "Notifications for new post sent to followers.",
                200,
                null
            ));
        }

        [HttpGet("my-notifications")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetMyNotifications([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] bool? isRead = null)
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            
            PaginatedList<NotificationEntity> notifications = await _uow.NotificationRepository.GetUserNotificationsPaginatedAsync(
                userId, pageNumber, pageSize, isRead);

            notifications.Code = 200;

            return Ok(notifications); 
            
        }

        [HttpPut("mark-as-read")]
        [EnableRateLimiting("UpdateItemPolicy")]
        public async Task<IActionResult> MarkNotificationsAsRead([FromBody] List<long> notificationIds)
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            
            if (notificationIds == null || !notificationIds.Any())
            {
                return BadRequest(new Response("fail", "No notification IDs provided.", 400, null));
            }

            int updatedCount = await _uow.NotificationRepository.MarkNotificationsAsReadAsync(notificationIds, userId);

            return Ok(new Response(
                "success",
                $"{updatedCount} notifications marked as read.",
                200,
                updatedCount
            ));
        }

        [HttpGet("unread-count")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetUnreadNotificationsCount()
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;

            int unreadCount = await _uow.NotificationRepository.GetUnreadNotificationsCountAsync(userId);

            return Ok(new Response(
                "success",
                "Unread notifications count fetched successfully.",
                200,
                unreadCount
            ));
        }
    }
}