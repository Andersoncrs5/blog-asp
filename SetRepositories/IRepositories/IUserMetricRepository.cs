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
        UserMetricEntity SumOrRedLikesOrDislikeGivenCountInComment(UserMetricEntity metric, SumOrRedEnum action, LikeOrDislike l);
        UserMetricEntity SumOrRedLikesOrDislikeGivenCountInPost(UserMetricEntity metric, SumOrRedEnum action, LikeOrDislike l);
        UserMetricEntity SumOrRedFollowersCount(UserMetricEntity metric, SumOrRedEnum action);
        UserMetricEntity SumOrRedFollowingCount(UserMetricEntity metric, SumOrRedEnum action);
        UserMetricEntity SumOrRedPostsCount(UserMetricEntity metric, SumOrRedEnum action);
        UserMetricEntity SumOrRedCommentsCount(UserMetricEntity metric, SumOrRedEnum action);
        UserMetricEntity SumOrRedSharesCount(UserMetricEntity metric, SumOrRedEnum action);
        UserMetricEntity SumOrRedSavedMediaCount(UserMetricEntity metric, SumOrRedEnum action);
        UserMetricEntity SumOrRedReportsReceivedCount(UserMetricEntity metric, SumOrRedEnum action);
        UserMetricEntity SumOrRedMediaUploadsCount(UserMetricEntity metric, SumOrRedEnum action);
        UserMetricEntity SumOrRedSavedPostsCount(UserMetricEntity metric, SumOrRedEnum action);
        UserMetricEntity SumOrRedSavedCommentsCount(UserMetricEntity metric, SumOrRedEnum action);
        UserMetricEntity SumOrRedEditedCount(UserMetricEntity metric, SumOrRedEnum action);
        UserMetricEntity SumOrRedProfileViews(UserMetricEntity metric, SumOrRedEnum action);
        UserMetricEntity SumOrRedLastLogin(UserMetricEntity metric, SumOrRedEnum action);
    }
}