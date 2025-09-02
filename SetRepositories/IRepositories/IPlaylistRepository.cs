using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.DTOs.Playlist;
using Blog.entities;
using Blog.utils;
using Blog.utils.enums;

namespace Blog.SetRepositories.IRepositories
{
    public interface IPlaylistRepository
    {
        Task<PlaylistEntity> Create(ApplicationUser user, CreatePlaylistDTO dto);
        Task<PlaylistEntity?> Get(ulong Id);
        Task<bool> ExistsByNameAndUserId(string userId, string Name);
        Task<PlaylistEntity> Update(PlaylistEntity play, UpdatePlaylistDTO dto);
        Task Delete(PlaylistEntity play);
        Task<PaginatedList<PlaylistEntity>> GetAllOfUserPaginated(ApplicationUser user, int pageNumber, int pageSize, bool showPublic = false);
        IQueryable<PlaylistEntity> GetAllOfUserQuery(ApplicationUser user);
        Task<PlaylistEntity> ChangeStatusIsPublic(PlaylistEntity play);
        Task SumOrReduceItemCount(PlaylistEntity play, SumOrRedEnum action);
    }
}