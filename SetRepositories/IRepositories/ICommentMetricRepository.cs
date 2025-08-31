using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.entities;
using Blog.entities.enums;
using Blog.utils.enums;

namespace Blog.SetRepositories.IRepositories
{
    public interface ICommentMetricRepository
    {
        Task<CommentMetricEntity?> Get(CommentEntity comment);
        Task<CommentMetricEntity> Create(CommentEntity comment);
        Task<CommentMetricEntity> SumOrRedLikeOrDislike(CommentMetricEntity metric, SumOrRedEnum action, LikeOrDislike ld);
        Task<CommentMetricEntity> SumOrRedReportCount(CommentMetricEntity metric, SumOrRedEnum action);
        Task<CommentMetricEntity> SumOrRedEditedTimes(CommentMetricEntity metric, SumOrRedEnum action);
        Task<CommentMetricEntity> SumOrRedFavoritesCount(CommentMetricEntity metric, SumOrRedEnum action);
        Task<CommentMetricEntity> SumOrRedRepliesCount(CommentMetricEntity metric, SumOrRedEnum action);
        Task<CommentMetricEntity> SumOrRedViewsCount(CommentMetricEntity metric, SumOrRedEnum action);
        
    }
}