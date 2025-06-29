using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blog.DTOs.Notification;
using blog.entities;
using Blog.entities;
using Blog.utils;

namespace blog.SetRepositories.IRepositories
{
    public interface INotificationRepository
    {
        Task<NotificationEntity> GetAsync(long Id);
        Task RemoveAsync(NotificationEntity noti);
        Task<NotificationEntity> SendNotification(CreateNotificationDTO dto, ApplicationUser recipientUser, string? senderUserId);
        Task SendNotificationToFollowersAboutNewPost(ApplicationUser postAuthor, PostEntity newPost);
        Task<int> MarkNotificationsAsReadAsync(List<long> notificationIds, string userId);
        Task<PaginatedList<NotificationEntity>> GetUserNotificationsPaginatedAsync(string? userId, int pageNumber, int pageSize, bool? isRead = null);
        Task<int> GetUnreadNotificationsCountAsync(string? userId);
    }
}