using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.entities;
using Blog.utils;

namespace Blog.SetRepositories.IRepositories
{
    public interface IPlaylistItemRepository
    {
        Task<PlaylistItemEntity> AddPostToPlaylist(PlaylistEntity playlist, PostEntity post, int? order = null);
        Task RemovePostFromPlaylist(PlaylistItemEntity playlistItem);
        Task<PlaylistItemEntity> Get(ulong Id);
        Task<PaginatedList<PlaylistItemEntity>> GetAllOfPlaylistPaginated(PlaylistEntity playlist, int pageNumber, int pageSize);
        Task<PlaylistItemEntity> UpdateOrder(PlaylistItemEntity playlistItem, int newOrder);
    }
}