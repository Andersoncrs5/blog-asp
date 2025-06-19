using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.entities;
using Blog.entities.enums;
using Blog.utils.enums;

namespace Blog.SetRepositories.IRepositories
{
    public interface IPostMetricRepository
    {
        Task<PostMetricEntity> Get(PostEntity post);
        Task<PostMetricEntity> Create(PostEntity post);
        Task<PostMetricEntity> SumOrRedLikeOrDislike(PostMetricEntity metric, SumOrRedEnum action, LikeOrDislike ld);
        Task<PostMetricEntity> SumOrRedShares(PostMetricEntity metric, SumOrRedEnum action);
        Task<PostMetricEntity> SumOrRedCommentCount(PostMetricEntity metric, SumOrRedEnum action);
        Task<PostMetricEntity> SumOrRedFavoriteCount(PostMetricEntity metric, SumOrRedEnum action);
        Task<PostMetricEntity> SumOrRedBookmarks(PostMetricEntity metric, SumOrRedEnum action);
        Task<PostMetricEntity> SumOrRedViewed(PostMetricEntity metric, SumOrRedEnum action);
        Task<PostMetricEntity> SumOrRedReportsReceivedCount(PostMetricEntity metric, SumOrRedEnum action);
        Task<PostMetricEntity> SumOrRedEditedCount(PostMetricEntity metric, SumOrRedEnum action);
        Task<PostMetricEntity> SumOrRedMediaCount(PostMetricEntity metric, SumOrRedEnum action);

    }
}