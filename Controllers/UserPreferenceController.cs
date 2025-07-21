using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using blog.DTOs.Preference;
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
            ApplicationUser user = await _uow.UserRepository.Get(userId);
            
            UserPreferenceEntity prefer = await _uow.UserPreferenceRepository.GetAsync(Id);
            await _uow.UserPreferenceRepository.RemoveAsync(prefer);

            UserMetricEntity metric = await _uow.UserMetricRepository.Get(user.Id);
            await _uow.UserMetricRepository.SumOrRedPreferenceCount(metric, Blog.utils.enums.SumOrRedEnum.REDUCE);

            return Ok(new Response(
                "success",
                "Preference removed!!",
                200,
                null
            ));
        }

        [HttpGet("get-all-user")] 
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfUserPaginated([FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value; 
            ApplicationUser user = await _uow.UserRepository.Get(userId);

            PaginatedList<UserPreferenceEntity> result = await _uow.UserPreferenceRepository.GetAllOfUserPaginatedAsync(user, pageNumber, pageSize);
            
            result.Code = 200;
            return Ok(result);
        }

        [HttpGet("{userId:required}/get-all-user")] 
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAllOfUserPaginated(string userId, [FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        {
            ApplicationUser user = await _uow.UserRepository.Get(userId);

            PaginatedList<UserPreferenceEntity> result = await _uow.UserPreferenceRepository.GetAllOfUserPaginatedAsync(user, pageNumber, pageSize);
            
            result.Code = 200;
            return Ok(result);
        }

        [HttpPost]
        [EnableRateLimiting("CreateItemPolicy")]
        public async Task<IActionResult> Save([FromBody] CreatePreferenceDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
                
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value; 
            ApplicationUser user = await _uow.UserRepository.Get(userId);
            await _uow.CategoryRepository.Get(dto.CategoryId);
            UserPreferenceEntity prefer = await _uow.UserPreferenceRepository.SaveAsync(dto, user);

            UserMetricEntity metric = await _uow.UserMetricRepository.Get(user.Id);
            await _uow.UserMetricRepository.SumOrRedPreferenceCount(metric, Blog.utils.enums.SumOrRedEnum.SUM);

            return Ok(new Response(
                "success",
                "Preference added!!!",
                201,
                prefer
            ));
        }

        [HttpGet("{categoryId:required:long}")]
        [EnableRateLimiting("CheckExistsPolicy")]
        public async Task<IActionResult> GetPreferenceByCategoryAsync(long categoryId)
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value; 
            ApplicationUser user = await _uow.UserRepository.Get(userId);
            bool check = await _uow.UserPreferenceRepository.GetPreferenceByCategoryAsync(user, categoryId);

            return Ok(new Response(
                "success",
                check? "Category are already added": "Category are not added!",
                200,
                check
            ));
        }

    }
}