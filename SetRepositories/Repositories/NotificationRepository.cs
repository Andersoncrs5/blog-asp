using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blog.DTOs.Notification;
using blog.entities;
using blog.entities.enums;
using blog.SetRepositories.IRepositories;
using Blog.Context;
using Blog.entities;
using Blog.utils;
using Microsoft.EntityFrameworkCore;

namespace blog.SetRepositories.Repositories
{
    public class NotificationRepository: INotificationRepository
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public NotificationRepository(
            AppDbContext context, 
            IConfiguration configuration
            )
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<NotificationEntity?> GetAsync(long Id)
        {
            if (Id <= 0) 
                throw new ArgumentNullException(nameof(Id));

            NotificationEntity? noti = await _context.NotificationEntities.AsNoTracking()
                .FirstOrDefaultAsync(n => n.Id == Id);

            if (noti is null)
                return null;

            return noti;
        }

        public async Task RemoveAsync(NotificationEntity noti)
        {
            _context.NotificationEntities.Remove(noti);
            await _context.SaveChangesAsync();
        }

        public async Task<NotificationEntity> SendNotification(CreateNotificationDTO dto, ApplicationUser recipientUser, string? senderUserId)
        {
            if (string.IsNullOrEmpty(recipientUser.Id))
                throw new ArgumentNullException(nameof(recipientUser));

            NotificationEntity noti = new NotificationEntity
            {
                ApplicationUserId = recipientUser.Id, 
                Title = dto.Title,
                Content = dto.Content,
                NotificationType = dto.NotificationType,
                IsRead = false,
                RelatedEntityId = dto.RelatedEntityId.ToString(),
                LinkUrl = dto.LinkUrl,
                IconCssClass = dto.IconCssClass,
                SenderUserId = senderUserId,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _context.NotificationEntities.AddAsync(noti);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task SendNotificationToFollowersAboutNewPost(ApplicationUser postAuthor, PostEntity newPost)
        {
            List<FollowEntity> followersWithNotifications = await _context.FollowsEntities
                .AsNoTracking()
                .Where(f => f.FollowedId == postAuthor.Id && f.ReceiveNotifications == true)
                .ToListAsync();

            string? frontendPostBaseUrl = _configuration["FrontendSettings:PostBaseUrl"] ?? "no url";
            if (string.IsNullOrEmpty(frontendPostBaseUrl) || string.IsNullOrWhiteSpace(frontendPostBaseUrl) )
            {
                Console.WriteLine("Warning: 'FrontendSettings:PostBaseUrl' is not configured. Notifications will not have direct links.");
                throw new ArgumentNullException(nameof(frontendPostBaseUrl));
            }

            List<NotificationEntity> notificationsToSend = new List<NotificationEntity>();

            foreach (FollowEntity item in followersWithNotifications)
            {
                NotificationEntity noti = new NotificationEntity
                {
                    ApplicationUserId = item.FollowerId, 
                    Title = $"Novo post de {postAuthor.UserName}!",
                    NotificationType = NotificationTypeEnum.NewPostFromFollowed,
                    RelatedEntityId = newPost.Id.ToString(),
                    LinkUrl = frontendPostBaseUrl,
                    IconCssClass = "fa-newspaper", 
                    SenderUserId = postAuthor.Id,
                    Content = $"O usuário que você segue, {postAuthor.UserName}, publicou um novo post: \"{newPost.Title}\"", 
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                };

                notificationsToSend.Add(noti);
            }

            if (notificationsToSend.Any())
            {
                await _context.NotificationEntities.AddRangeAsync(notificationsToSend);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> MarkNotificationsAsReadAsync(List<long> notificationIds, string userId)
        {
            if (notificationIds == null || !notificationIds.Any()) return 0;

            if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId));

            List<NotificationEntity> notificationsToUpdate = await _context.NotificationEntities
                .Where(n => notificationIds.Contains(n.Id) && n.ApplicationUserId == userId && n.IsRead == false)
                .ToListAsync();

            foreach (NotificationEntity notification in notificationsToUpdate)
            {
                notification.IsRead = true;
                notification.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return notificationsToUpdate.Count;
        }

        public async Task<PaginatedList<NotificationEntity>> GetUserNotificationsPaginatedAsync(string? userId, int pageNumber, int pageSize, bool? isRead = null)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            IQueryable<NotificationEntity> query = _context.NotificationEntities
                .AsNoTracking()
                .Where(n => n.ApplicationUserId == userId);

            if (isRead.HasValue)
            {
                query = query.Where(n => n.IsRead == isRead.Value);
            }

            query = query.OrderByDescending(n => n.CreatedAt);

            return await PaginatedList<NotificationEntity>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<int> GetUnreadNotificationsCountAsync(string? userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            return await _context.NotificationEntities
                .AsNoTracking()
                .CountAsync(n => n.ApplicationUserId == userId && n.IsRead == false);
        }
    }
}