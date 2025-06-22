using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.entities;
using Blog.entities.enums;
using Blog.utils.enums;

namespace Blog.SetRepositories.IRepositories
{
    public interface IUserMetricRepository
    {
        Task<UserMetricEntity> Get(string? userId);
        Task<UserMetricEntity> Create(string userId);
        Task<UserMetricEntity> SumOrRedLikesOrDislikeGivenCountInComment(UserMetricEntity metric, SumOrRedEnum action, LikeOrDislike l);
        Task<UserMetricEntity> SumOrRedLikesOrDislikeGivenCountInPost(UserMetricEntity metric, SumOrRedEnum action, LikeOrDislike l);
        Task<UserMetricEntity> SumOrRedFollowersCount(UserMetricEntity metric, SumOrRedEnum action);
        Task<UserMetricEntity> SumOrRedFollowingCount(UserMetricEntity metric, SumOrRedEnum action);
        Task<UserMetricEntity> SumOrRedPostsCount(UserMetricEntity metric, SumOrRedEnum action);
        Task<UserMetricEntity> SumOrRedCommentsCount(UserMetricEntity metric, SumOrRedEnum action);
        Task<UserMetricEntity> SumOrRedSharesCount(UserMetricEntity metric, SumOrRedEnum action);
        Task<UserMetricEntity> SumOrRedSavedMediaCount(UserMetricEntity metric, SumOrRedEnum action);
        Task<UserMetricEntity> SumOrRedReportsReceivedCount(UserMetricEntity metric, SumOrRedEnum action);
        Task<UserMetricEntity> SumOrRedMediaUploadsCount(UserMetricEntity metric, SumOrRedEnum action);
        Task<UserMetricEntity> SumOrRedSavedPostsCount(UserMetricEntity metric, SumOrRedEnum action);
        Task<UserMetricEntity> SumOrRedSavedCommentsCount(UserMetricEntity metric, SumOrRedEnum action);
        Task<UserMetricEntity> SumOrRedEditedCount(UserMetricEntity metric, SumOrRedEnum action);
        Task<UserMetricEntity> SumOrRedProfileViews(UserMetricEntity metric, SumOrRedEnum action);
        Task<UserMetricEntity> SumOrRedPlaylistCount(UserMetricEntity metric, SumOrRedEnum action);
        Task<UserMetricEntity> SumOrRedLastLogin(UserMetricEntity metric, SumOrRedEnum action);
        Task<UserMetricEntity> SumOrRedPreferenceCount(UserMetricEntity metric, SumOrRedEnum action);
    }
}