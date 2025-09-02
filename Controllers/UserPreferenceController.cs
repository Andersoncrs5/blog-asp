using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using blog.DTOs.Preference;
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
    public class UserPreferenceController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public UserPreferenceController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpDelete("{Id:required:long}")]
        [EnableRateLimiting("DeleteItemPolicy")]
        public async Task<IActionResult> Remove(long Id)
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

            ApplicationUser? user = await _uow.UserRepository.Get(userId);
            if (user == null)
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

            UserPreferenceEntity? prefer = await _uow.UserPreferenceRepository.GetAsync(Id);
            if (prefer == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Message = "Config User not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            await _uow.UserPreferenceRepository.RemoveAsync(prefer);

            UserMetricEntity? metric = await _uow.UserMetricRepository.Get(user.Id);
            if (metric == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Datetime = DateTimeOffset.Now,
                    Message = "User metric not found",
                    Status = false
                });
            }

            await _uow.UserMetricRepository.SumOrRedPreferenceCount(metric, Blog.utils.enums.SumOrRedEnum.REDUCE);

            return StatusCode(200, new ResponseBody<UserPreferenceEntity>
            {
                Status = true,
                Message = "Preference removed!!",
                Code = 200,
                Body = null,
                Datetime = DateTimeOffset.Now,
            });
        }

        [HttpGet("get-all-user")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfUserPaginated([FromQuery] PreferenceFilterDTO filter)
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

            ApplicationUser? user = await _uow.UserRepository.Get(userId);
            if (user == null)
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

            IQueryable<UserPreferenceEntity> result = _uow.UserPreferenceRepository.GetAllOfUserPaginatedAsync(user);

            IQueryable<UserPreferenceEntity> resultFilter = PreferenceQueryFilter.ApplyFilters(result, filter);

            PaginatedList<UserPreferenceEntity> page = await PaginatedList<UserPreferenceEntity>.CreateAsync(resultFilter, filter.PageNumber, filter.PageSize);

            return Ok(new ResponseBody<PaginatedList<UserPreferenceEntity>>
            {
                Status = true,
                Message = "All User Preference",
                Code = 200,
                Body = page,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpGet("{userId:required}/get-all-user")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfUserPaginated(string userId, [FromQuery] PreferenceFilterDTO filter)
        {
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

            ApplicationUser? user = await _uow.UserRepository.Get(userId);

            if (user == null)
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

            IQueryable<UserPreferenceEntity> result = _uow.UserPreferenceRepository.GetAllOfUserPaginatedAsync(user);

            IQueryable<UserPreferenceEntity> resultFilter = PreferenceQueryFilter.ApplyFilters(result, filter);

            PaginatedList<UserPreferenceEntity> page = await PaginatedList<UserPreferenceEntity>.CreateAsync(resultFilter, filter.PageNumber, filter.PageSize);

            return Ok(new ResponseBody<PaginatedList<UserPreferenceEntity>>
            {
                Status = true,
                Message = "All User Preference",
                Code = 200,
                Body = page,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpPost]
        [EnableRateLimiting("CreateItemPolicy")]
        public async Task<IActionResult> Save([FromBody] CreatePreferenceDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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

            ApplicationUser? user = await _uow.UserRepository.Get(userId);
            if (user == null)
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

            CategoryEntity? category = await _uow.CategoryRepository.Get(dto.CategoryId);
            if (category == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Message = "Category not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            bool checkExists = await _uow.UserPreferenceRepository.Exists(dto.CategoryId);
            if (checkExists)
            {
                return Conflict(new ResponseBody<string>
                {
                    Body = null,
                    Code = 409,
                    Message = "Category already are added",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            UserPreferenceEntity prefer = await _uow.UserPreferenceRepository.SaveAsync(dto, user);

            UserMetricEntity? metric = await _uow.UserMetricRepository.Get(user.Id);
            if (metric == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Datetime = DateTimeOffset.Now,
                    Message = "User metric not found",
                    Status = false
                });
            }

            await _uow.UserMetricRepository.SumOrRedPreferenceCount(metric, Blog.utils.enums.SumOrRedEnum.SUM);

            return StatusCode(201, new ResponseBody<UserPreferenceEntity>
            {
                Status = true,
                Message = "Preference added!!!",
                Code = 201,
                Body = prefer,
                Datetime = DateTimeOffset.Now,
            });
        }

        [HttpGet("{categoryId:required:long}")]
        [EnableRateLimiting("CheckExistsPolicy")]
        public async Task<IActionResult> GetPreferenceByCategoryAsync(long categoryId)
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

            ApplicationUser? user = await _uow.UserRepository.Get(userId);
            if (user == null)
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

            bool check = await _uow.UserPreferenceRepository.GetPreferenceByCategoryAsync(user, categoryId);

            return StatusCode(200, new ResponseBody<bool>
            {
                Status = true,
                Message = check ? "Category are already added" : "Category are not added!",
                Code = 200,
                Body = check,
                Datetime = DateTimeOffset.Now,
            });
        }

    }
}